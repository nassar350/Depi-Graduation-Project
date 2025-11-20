using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.DTOs.Users;

namespace Eventify.Service.DTOs.Events
{
    public class EventDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }= string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[] Photo { get; set; }

        public int OrganizerID { get; set; }

        public UserDto Organizer { get; set; }= new UserDto();

        public List<CategoryDto> Categories { get; set; } = new();

        public List<UserDto> Attendees { get; set; }= new();

        public List<TicketDto> Tickets { get; set; } = new();

        public List<BookingDto> Bookings { get; set; } = new();
    }

}
