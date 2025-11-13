using AutoMapper;
using Eventify.Service.DTOs.Payments;
using Eventify.Core.Entities;

namespace Eventify.Service.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Payment, PaymentDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Booking.User.Email))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Booking.User.PhoneNumber));

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime ?? DateTime.UtcNow));

            CreateMap<UpdatePaymentDto, Payment>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
