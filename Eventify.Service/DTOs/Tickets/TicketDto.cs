namespace Eventify.Service.DTOs.Tickets
{
    public class TicketDto
    {
        public int ID { get; set; }

        public string Place { get; set; }= string.Empty;

        public string Type { get; set; }= string.Empty;

        public int CategoryId { get; set; }

        public decimal Price { get; set; }
        public int EventId { get; set; }

        public int? BookingId { get; set; }
    }

}
