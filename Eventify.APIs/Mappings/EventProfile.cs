using AutoMapper;
using Eventify.APIs.DTOs.Events;
using Eventify.Core.Entities;

namespace Eventify.APIs.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>();

            CreateMap<CreateEventDto, Event>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<UpdateEventDto, Event>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Event, EventDetailsDto>()
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Organizer))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.EventsAttendedByUsers.Select(e => e.User)))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets))
                .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.Bookings));
        }
    }

}
