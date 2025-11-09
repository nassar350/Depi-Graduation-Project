namespace Eventify.APIs.DTOs.Tickets
{
    public class TicketDto
    {
        public int ID { get; set; }

        public string Place { get; set; }

        public string Type { get; set; }

        public int CategoryId { get; set; }

        public int EventId { get; set; }

        public int? BookingId { get; set; }
    }

}
