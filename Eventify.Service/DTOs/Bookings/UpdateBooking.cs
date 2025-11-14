using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Bookings
{
    public class UpdateBookingDto
    {
        public TicketStatus? Status { get; set; }

        public int? TicketsNum { get; set; }
    }

}
