using AutoMapper;
using Eventify.Service.DTOs.Bookings;
using Eventify.Core.Entities;

namespace Eventify.Service.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>();

            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateBookingDto, Booking>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Booking, BookingDetailsDto>();
        }
    }

}
