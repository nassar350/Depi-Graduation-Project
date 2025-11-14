namespace Eventify.Service.DTOs.Tickets
{
    public class UpdateTicketDto
    {
        public string? Place { get; set; }

        public string? Type { get; set; }

        public int? CategoryId { get; set; }

        public int? EventId { get; set; }

        public int? BookingId { get; set; }
    }

}
