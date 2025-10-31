﻿using AutoMapper;
using Eventify.APIs.DTOs.Payments;
using Eventify.Core.Entities;

namespace Eventify.APIs.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Payment, PaymentDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime ?? DateTime.UtcNow));

            CreateMap<UpdatePaymentDto, Payment>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
