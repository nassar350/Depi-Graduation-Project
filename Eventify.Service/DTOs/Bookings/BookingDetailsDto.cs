using Eventify.APIs.DTOs.Events;
using Eventify.APIs.DTOs.Payments;
using Eventify.APIs.DTOs.Tickets;
using Eventify.APIs.DTOs.Users;
using Eventify.Core.Enums;

namespace Eventify.APIs.DTOs.Bookings
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public TicketStatus Status { get; set; }

        public int TicketsNum { get; set; }

        public UserDto User { get; set; }

        public EventDto Event { get; set; }

        public PaymentDto Payment { get; set; }

        public List<TicketDto> Tickets { get; set; }
    }

}
