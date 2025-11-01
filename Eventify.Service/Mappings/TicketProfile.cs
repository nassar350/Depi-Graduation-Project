using AutoMapper;
using Eventify.APIs.DTOs.Tickets;
using Eventify.Core.Entities;

namespace Eventify.APIs.Mappings
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketDto>();

            CreateMap<Ticket, TicketDetailsDto>();

            CreateMap<CreateTicketDto, Ticket>();

            CreateMap<UpdateTicketDto, Ticket>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

}
