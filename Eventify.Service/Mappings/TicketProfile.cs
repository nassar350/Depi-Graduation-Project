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

            CreateMap<Ticket, TicketDetailsDto>();

            CreateMap<CreateTicketDto, Ticket>();

            CreateMap<UpdateTicketDto, Ticket>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

}
