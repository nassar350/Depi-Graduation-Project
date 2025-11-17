using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Tickets
{
    public class UpdateTicketDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Place must be between 3 and 100 characters")]
        public string? Place { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Type must be between 2 and 50 characters")]
        public string? Type { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Event ID must be a positive number")]
        public int? EventId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be a positive number")]
        public int? BookingId { get; set; }
    }

}
