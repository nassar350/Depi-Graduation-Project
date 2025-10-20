using Eventify.APIs.DTOs.Bookings;
using Eventify.APIs.DTOs.Categories;
using Eventify.APIs.DTOs.Events;

namespace Eventify.APIs.DTOs.Tickets
{
    public class TicketDetailsDto
    {
        public int ID { get; set; }

        public string Place { get; set; }

        public string Type { get; set; }

        public CategoryDto Category { get; set; }

        public EventDto Event { get; set; }

        public BookingDto? Booking { get; set; }
    }

}
