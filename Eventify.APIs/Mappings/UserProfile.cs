using AutoMapper;
using Eventify.APIs.DTOs.Users;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Eventify.APIs.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<User, UserDetailsDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.CreatedEvents, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.AttendingEvents, opt => opt.MapFrom(src => src.UserAttendEvents.Select(ua => ua.Event)))
                .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.Bookings));

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.User));

            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

}
