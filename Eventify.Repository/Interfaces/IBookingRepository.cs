using Eventify.Core.Entities;

namespace Eventify.Repository.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
  
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> AddAsync(Booking booking);
    
    Task<bool> UpdateAsync(int id, Booking booking);
    Task<bool> DeleteAsync(int id);
}