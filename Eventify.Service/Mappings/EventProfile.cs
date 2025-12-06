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
               .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
               .ForMember(dest => dest.Categories, opt => opt.Ignore()) // map CategoryIds manually in service
                .ForMember(dest => dest.Organizer, opt => opt.Ignore())
                .ForMember(dest => dest.EventsAttendedByUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.Name : "Unknown"))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl ?? ""))
              
                .ForMember(dest => dest.AvailableTickets,
                    opt => opt.MapFrom(src => src.Categories != null ? src.Categories.Sum(c => c.Seats - c.Booked) : 0))
                .ForMember(dest => dest.IsUpcoming,
                    opt => opt.MapFrom(src => src.StartDate > DateTime.UtcNow))
                .ForMember(dest => dest.EventCategory, opt => opt.MapFrom(src => src.EventCategory.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => GetEventStatus(src.StartDate, src.EndDate)));

            CreateMap<Event, EventDetailsDto>()
                .ForMember(dest => dest.EventCategory, opt => opt.MapFrom(src => src.EventCategory.ToString()))
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.Attendees,
                    opt => opt.MapFrom(src => src.EventsAttendedByUsers.Select(a => a.User)))
                .ForMember(dest => dest.Tickets,
                    opt => opt.MapFrom(src => src.Tickets))
                .ForMember(dest => dest.Bookings,
                    opt => opt.Ignore());

            CreateMap<UpdateEventDto, Event>()
               .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
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

        private static string GetEventStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.UtcNow;
            if (now < startDate)
                return "Upcoming";
            else if (now >= startDate && now <= endDate)
                return "Ongoing";
            else
                return "Completed";
        }
    }
}
