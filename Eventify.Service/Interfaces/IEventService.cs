using Eventify.Core.Entities;
using Eventify.Service.DTOs.Events;
using Eventify.Service.Helpers;

namespace Eventify.Service.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<ServiceResult<EventDetailsDto>> GetByIdAsync(int id); 
        Task<ServiceResult<EventDto>> CreateAsync(CreateEventDto dto, string userId);
        Task<bool> UpdateAsync(int id, UpdateEventDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ServiceResult<IEnumerable<EventDto>>> GetUpcomingEventsAsync(int take = 10);
        Task<ServiceResult<IEnumerable<EventDto>>> GetEventsByOrganizerAsync(int organizerId);
        Task<ServiceResult<IEnumerable<EventDto>>> SearchEventsAsync(string searchTerm);
    }
}
