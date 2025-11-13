using AutoMapper;
using Eventify.Service.DTOs.Events;
using Eventify.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Eventify.Service.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<CreateEventDto, Event>()
                .ForMember(dest => dest.Photo,
                    opt => opt.MapFrom(src => ConvertFormFileToByteArray(src.Photo))) // handle IFormFile -> byte[]
                .ForMember(dest => dest.Categories, opt => opt.Ignore()) // map CategoryIds manually in service
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.EventsAttendedByUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<Event, EventDto>();

            CreateMap<Event, EventDetailsDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.Attendees,
                    opt => opt.MapFrom(src => src.EventsAttendedByUsers.Select(a => a.User)))
                .ForMember(dest => dest.Bookings,
                    opt => opt.Ignore());

            CreateMap<UpdateEventDto, Event>()
                .ForMember(dest => dest.Photo,
                    opt => opt.MapFrom(src => ConvertFormFileToByteArray(src.Photo)))
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));
        }

        private static byte[]? ConvertFormFileToByteArray(IFormFile? file)
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
