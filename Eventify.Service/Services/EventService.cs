using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Eventify.Service.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var events = await _unitOfWork._eventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<ServiceResult<EventDetailsDto>> GetByIdAsync(int id)
        {
            try
            {
                var eventEntity = await _unitOfWork._eventRepository.GetEventWithDetailsAsync(id);
                if (eventEntity == null)
                {
                    return ServiceResult<EventDetailsDto>.Fail("NotFound", $"Event with ID {id} not found.");
                }

                var eventDetails = _mapper.Map<EventDetailsDto>(eventEntity);
                return ServiceResult<EventDetailsDto>.Ok(eventDetails);
            }
            catch (Exception ex)
            {
                return ServiceResult<EventDetailsDto>.Fail("Exception", $"An error occurred while fetching event details: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EventDto>> CreateAsync(CreateEventDto dto, string id)
        {
            var errors = new List<ValidationError>();
            if (dto.EndDate <= dto.StartDate)
            {
                errors.Add(new ValidationError { Field = "EndDate", Message = "End Date must be after the Start Date" });
            }
            if (dto.StartDate <= DateTime.UtcNow)
            {
                errors.Add(new ValidationError { Field = "StartDate", Message = "Start Date must be in the future" });
            }
            if (errors.Any())
                return ServiceResult<EventDto>.Fail(errors);

            var Categories = JsonConvert.DeserializeObject<List<CategoryCreationByEventDto>>(dto.CategoriesJson);

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var ev = _mapper.Map<Event>(dto);
                ev.OrganizerID = int.Parse(id);
                var curEvent = await _unitOfWork._eventRepository.AddAsync(ev);
                await _unitOfWork.SaveChangesAsync();
                var CategoriesToCreate = new List<CreateCategoryDto>();
                var ticketsToCreate = new List<CreateTicketDto>();
                if (Categories is not null && Categories.Any())
                {
                    foreach (var category in Categories)
                    {
                        CategoriesToCreate.Add(new CreateCategoryDto(curEvent.Id, category.Title, category.Seats , category.TicketPrice));
                    }
                }
                var mappedCateories = _mapper.Map<List<Category>>(CategoriesToCreate);
                await _unitOfWork._categoryRepository.AddRangeAsync(mappedCateories);
                await _unitOfWork.SaveChangesAsync();
                foreach (var category in mappedCateories)
                {
                    for (int i = 0; i < category.Seats; i++)
                    {
                        ticketsToCreate.Add(new CreateTicketDto(curEvent.Address, category.Title, category.Id, curEvent.Id , category.TicketPrice));
                    }
                }
                var mappedTickets = _mapper.Map<List<Ticket>>(ticketsToCreate);
                await _unitOfWork._ticketRepository.AddRangeAsync(mappedTickets);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                var CurDto = _mapper.Map<EventDto>(ev);
                return ServiceResult<EventDto>.Ok(CurDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                errors.Add(new ValidationError { Field = "Exception", Message = ex.Message });
                return ServiceResult<EventDto>.Fail(errors);
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateEventDto dto)
        {
            var ev = await _unitOfWork._eventRepository.GetByIdAsync(id);
            if (ev == null) return false;
            _mapper.Map(dto, ev);
            _unitOfWork._eventRepository.Update(ev);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ev = await _unitOfWork._eventRepository.GetByIdAsync(id);
            if (ev == null) return false;
            _unitOfWork._eventRepository.Delete(ev);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<ServiceResult<IEnumerable<EventDto>>> GetUpcomingEventsAsync(int take = 10)
        {
            try
            {
                var allEvents = await _unitOfWork._eventRepository.GetAllAsync();
                var upcomingEvents = allEvents
                    .Where(e => e.StartDate > DateTime.UtcNow)
                    .OrderBy(e => e.StartDate)
                    .Take(take)
                    .ToList();

                if (!upcomingEvents.Any())
                {
                    return ServiceResult<IEnumerable<EventDto>>.Fail("NoEvents", "No upcoming events found.");
                }

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(upcomingEvents);
                return ServiceResult<IEnumerable<EventDto>>.Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<EventDto>>.Fail("Exception", $"An error occurred while fetching upcoming events: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<EventDto>>> GetEventsByOrganizerAsync(int organizerId)
        {
            try
            {
                var allEvents = await _unitOfWork._eventRepository.GetAllAsync();
                var organizerEvents = allEvents
                    .Where(e => e.OrganizerID == organizerId)
                    .OrderByDescending(e => e.StartDate)
                    .ToList();

                if (!organizerEvents.Any())
                {
                    return ServiceResult<IEnumerable<EventDto>>.Fail("NoEvents", $"No events found for organizer with ID {organizerId}.");
                }

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(organizerEvents);
                return ServiceResult<IEnumerable<EventDto>>.Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<EventDto>>.Fail("Exception", $"An error occurred while fetching organizer events: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<EventDto>>> SearchEventsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ServiceResult<IEnumerable<EventDto>>.Fail("InvalidSearch", "Search term cannot be empty.");
                }

                var allEvents = await _unitOfWork._eventRepository.GetAllAsync();
                var matchingEvents = allEvents
                    .Where(e => e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               e.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Where(e => e.StartDate > DateTime.UtcNow) 
                    .OrderBy(e => e.StartDate)
                    .ToList();

                if (!matchingEvents.Any())
                {
                    return ServiceResult<IEnumerable<EventDto>>.Fail("NoResults", $"No events found matching '{searchTerm}'.");
                }

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(matchingEvents);
                return ServiceResult<IEnumerable<EventDto>>.Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<EventDto>>.Fail("Exception", $"An error occurred during search: {ex.Message}");
            }
        }
    }
}
