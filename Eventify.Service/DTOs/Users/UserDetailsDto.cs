using Eventify.APIs.DTOs.Bookings;
using Eventify.APIs.DTOs.Events;

namespace Eventify.APIs.DTOs.Users
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
