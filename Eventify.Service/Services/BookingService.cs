using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Events;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services;

public class BookingService : IBookingService
{
   
        private readonly IBookingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

    public BookingService(IBookingRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

        public async Task<IEnumerable<BookingDto>> GetAllAsync()
        {
            var bookings = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        
        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var booking = await _repo.GetByIdAsync(id);
            return _mapper.Map<BookingDto>(booking);
        }

        

        public async Task<BookingDto> CreateAsync(CreateBookingDto booking)
        {
            var user = _unitOfWork._userRepository.GetUserByEmail(booking.EmailAddress);
            booking.UserId = user.Id;

            var entity = _mapper.Map<Booking>(booking);
            var created = await _repo.AddAsync(entity); 
            return _mapper.Map<BookingDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookingDto updatedBooking)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            var updatedEntity = _mapper.Map(updatedBooking , existing);
            
            return await _repo.UpdateAsync(id, _mapper.Map<Booking>(updatedEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);

        }

    public async Task<bool> RefundThisBooking(int bookingId)
    {
        var booking = await _repo.GetByIdAsync(bookingId);

        if (booking == null)
            return false;

        foreach(var ticket in booking.Tickets)
        {
            ticket.BookingId = null;
        }

        var category = await _unitOfWork._categoryRepository.GetByIdAsync(booking.Tickets.First().CategoryId);
        category.Booked -= booking.TicketsNum;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<ServiceResult<IEnumerable<BookingDetailsDto>>> GetByUserId(int userId)
    {
        try
        {
            var bookings = await _repo.GetDetailedByUserId(userId);
            if (bookings == null || !bookings.Any())
            {
                return ServiceResult<IEnumerable<BookingDetailsDto>>.Ok(Enumerable.Empty<BookingDetailsDto>());
            }
            
            var BookingsDetailDto = _mapper.Map<IEnumerable<BookingDetailsDto>>(bookings);
            
            // Event data is now automatically mapped via the Event navigation property
            // No need to manually look it up
            
            return ServiceResult<IEnumerable<BookingDetailsDto>>.Ok(BookingsDetailDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<BookingDetailsDto>>.Fail("Error", $"Error retrieving bookings: {ex.Message}");
        }
    }
}