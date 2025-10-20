using Eventify.APIs.DTOs.Bookings;
using Eventify.APIs.DTOs.Categories;
using Eventify.APIs.DTOs.Tickets;
using Eventify.APIs.DTOs.Users;

namespace Eventify.APIs.DTOs.Events
{
    public class EventDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[] Photo { get; set; }

        public int OrganizerID { get; set; }

        public UserDto Organizer { get; set; }

        public List<CategoryDto> Categories { get; set; }

        public List<UserDto> Attendees { get; set; }

        public List<TicketDto> Tickets { get; set; }

        public List<BookingDto> Bookings { get; set; }
    }

}
