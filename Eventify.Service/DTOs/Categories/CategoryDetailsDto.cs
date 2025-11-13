using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Tickets;

namespace Eventify.Service.DTOs.Categories
{
    public class CategoryDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Seats { get; set; }

        public int Booked { get; set; }

        public int EventId { get; set; }

        public EventDto Event { get; set; }

        public List<TicketDto> Tickets { get; set; }
    }

}
