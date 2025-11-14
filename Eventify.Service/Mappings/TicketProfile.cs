using AutoMapper;
using Eventify.Service.DTOs.Tickets;
using Eventify.Core.Entities;

namespace Eventify.Service.Mappings
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketDto>();

            CreateMap<Ticket, TicketDetailsDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => src.Booking));

            CreateMap<CreateTicketDto, Ticket>();

            CreateMap<UpdateTicketDto, Ticket>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

}
