using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services;

public class BookingService : IBookingService
{
   
        private readonly IBookingRepository _repo;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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
            var entity = _mapper.Map<Booking>(booking);
            var created = await _repo.AddAsync(entity); 
            return _mapper.Map<BookingDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookingDto updatedBooking)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            var updatedEntity = _mapper.Map(updatedBooking , existing) ;
            
            return await _repo.UpdateAsync(id, _mapper.Map<Booking>(updatedEntity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);

        }
}