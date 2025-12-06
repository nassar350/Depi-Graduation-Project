using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Payments;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.DTOs.Users;
using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Bookings
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public BookingStatus Status { get; set; }

        public int TicketsNum { get; set; }

        public UserDto User { get; set; } = new();

        public EventDto Event { get; set; }= new();

        public PaymentDto Payment { get; set; }= new();

        public List<TicketDto> Tickets { get; set; }= new();
    }

}
