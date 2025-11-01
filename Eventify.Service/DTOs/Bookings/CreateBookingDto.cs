using System.ComponentModel.DataAnnotations;

namespace Eventify.APIs.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int TicketsNum { get; set; }

    }

}
