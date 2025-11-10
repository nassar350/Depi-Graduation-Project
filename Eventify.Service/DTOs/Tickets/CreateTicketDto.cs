using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Tickets
{
    public class CreateTicketDto
    {
        [Required]
        public string Place { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }

        public int? BookingId { get; set; }
    }

}
