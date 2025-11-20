using Eventify.Core.Entities;

namespace Eventify.Repository.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task<Event> AddAsync(Event entity);
        void Update(Event entity);
        void Delete(Event entity);
        Task<bool> ExistsAsync(int id);

        // New method for detailed event information
        Task<Event?> GetEventWithDetailsAsync(int id);
    }
}
