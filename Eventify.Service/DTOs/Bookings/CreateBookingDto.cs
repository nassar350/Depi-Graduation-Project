using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Event ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Event ID must be a positive number")]
        public int EventId { get; set; }

       //seeeeeeeeeeeeeeeeeeeeeee
        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(1, 100, ErrorMessage = "You can book between 1 and 100 tickets per request")]
        public int TicketsNum { get; set; }

    }

}
