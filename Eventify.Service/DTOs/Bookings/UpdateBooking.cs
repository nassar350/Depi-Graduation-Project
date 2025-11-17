using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Bookings
{
    public class UpdateBookingDto
    { //seeeeeeeeeeeeeeeeeeeeeeeee
        [EnumDataType(typeof(TicketStatus), ErrorMessage = "Invalid status value")]
        public TicketStatus? Status { get; set; }

        [Range(1, 100, ErrorMessage = "Tickets number must be between 1 and 100")]
        public int? TicketsNum { get; set; }
    }

}
