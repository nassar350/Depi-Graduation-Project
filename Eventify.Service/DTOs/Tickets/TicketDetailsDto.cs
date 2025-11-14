using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;

namespace Eventify.Service.DTOs.Tickets
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
