using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Events;

namespace Eventify.Service.DTOs.Users
{
    public class UserDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; }

        public List<EventDto> CreatedEvents { get; set; }

        public List<EventDto> AttendingEvents { get; set; }

        public List<BookingDto> Bookings { get; set; }
    }

}
