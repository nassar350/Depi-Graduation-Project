using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Service.DTOs.Admin;

namespace Eventify.Service.Mappings
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // User mappings
            CreateMap<User, AdminUserDto>()
                .ForMember(dest => dest.EventsCreated, opt => opt.Ignore())
                .ForMember(dest => dest.BookingsCount, opt => opt.Ignore());

            CreateMap<AdminUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore())
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())
                .ForMember(dest => dest.UserAttendEvents, opt => opt.Ignore());

            // Event mappings
            CreateMap<Event, AdminEventDto>()
                .ForMember(dest => dest.OrganizerName, opt => opt.Ignore())
                .ForMember(dest => dest.BookedTickets, opt => opt.Ignore())
                .ForMember(dest => dest.Revenue, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // Booking mappings
            CreateMap<Booking, AdminBookingDto>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
                .ForMember(dest => dest.EventName, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());

            // Payment mappings
            CreateMap<Payment, AdminPaymentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.EventName, opt => opt.Ignore());

            // Category mappings
            CreateMap<Category, AdminCategoryDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.TicketPrice))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Seats))
                .ForMember(dest => dest.EventName, opt => opt.Ignore());

            CreateMap<AdminCategoryDto, Category>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());
        }
    }
}
