using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Events;

namespace Eventify.Service.DTOs.Users
{
    public class UserDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public List<EventDto> CreatedEvents { get; set; } = new();

        public List<EventDto> AttendingEvents { get; set; } = new();

        public List<BookingDto> Bookings { get; set; } = new();
    }

}
