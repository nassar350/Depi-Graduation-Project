using Eventify.Core.Enums;

namespace Eventify.APIs.DTOs.Bookings
{
    public class BookingDto
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public TicketStatus Status { get; set; }

        public int TicketsNum { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }
    }

}
