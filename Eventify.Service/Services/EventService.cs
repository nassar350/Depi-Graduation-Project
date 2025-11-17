using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Helpers;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IMapper mapper , IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EventDto>> GetAllAsync()
    {
        var events = await _unitOfWork._eventRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto> GetByIdAsync(int id)
    {
        var ev = await _unitOfWork._eventRepository.GetByIdAsync(id);
        return _mapper.Map<EventDto>(ev);
    }

    public async Task<ServiceResult<EventDto>> CreateAsync(CreateEventDto dto, string id)
    {
        var errors = new List<ValidationError>();
        if (dto.EndDate <= dto.StartDate)
        {
            errors.Add(new ValidationError { Field = "EndDate", Message = "End Date must be after the Start Date" });
        }
        if (dto.StartDate == DateTime.UtcNow)
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
                    CategoriesToCreate.Add(new CreateCategoryDto(int.Parse(id), category.Title, category.Seats));
                }
            }
            var mappedCateories = _mapper.Map<List<Category>>(CategoriesToCreate);
            await _unitOfWork._categoryRepository.AddRangeAsync(mappedCateories);
            await _unitOfWork.SaveChangesAsync();
            foreach (var category in mappedCateories)
            {
                for (int i = 0; i < category.Seats; i++)
                {
                    ticketsToCreate.Add(new CreateTicketDto(curEvent.Address, category.Title, category.Id, curEvent.Id));
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
}
