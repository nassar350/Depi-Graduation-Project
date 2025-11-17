using Eventify.Core.Entities;
using Eventify.Service.DTOs.Events;
using Eventify.Service.Helpers;


public interface IEventService
{
    Task<IEnumerable<EventDto>> GetAllAsync();
    Task<EventDto> GetByIdAsync(int id);
    Task<ServiceResult<EventDto>> CreateAsync(CreateEventDto dto , string userId);
    Task<bool> UpdateAsync(int id, UpdateEventDto dto);
    Task<bool> DeleteAsync(int id);
  
}
