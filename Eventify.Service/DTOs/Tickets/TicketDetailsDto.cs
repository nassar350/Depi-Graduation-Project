using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;

namespace Eventify.Service.DTOs.Tickets
{
    public class TicketDetailsDto
    {
        public int ID { get; set; }

        public string Place { get; set; }= string.Empty;

        public string Type { get; set; }= string.Empty;

        public CategoryDto Category { get; set; }= new();

        public EventDto Event { get; set; }= new();

        public BookingDto? Booking { get; set; }
    }

}
