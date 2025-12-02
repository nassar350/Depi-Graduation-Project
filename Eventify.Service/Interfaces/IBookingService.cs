using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Helpers;

namespace Eventify.Service.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<BookingDto?> GetByIdAsync(int id);
    
    Task<BookingDto> CreateAsync(CreateBookingDto booking);
    Task<bool> UpdateAsync(int id, UpdateBookingDto updatedBooking);
    Task<bool> DeleteAsync(int id);
    Task<ServiceResult<IEnumerable<BookingDetailsDto>>> GetByUserId(int userId);
    Task<bool> RefundThisBooking(int bookingId);
}