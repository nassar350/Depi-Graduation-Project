using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Bookings
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
