using Eventify.Service.DTOs.Bookings;

namespace Eventify.Service.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<BookingDto?> GetByIdAsync(int id);
    Task<BookingDto> CreateAsync(CreateBookingDto booking);
    Task<bool> UpdateAsync(int id, UpdateBookingDto updatedBooking);
    Task<bool> DeleteAsync(int id);
    Task<bool> RefundThisBooking(int bookingId);
}