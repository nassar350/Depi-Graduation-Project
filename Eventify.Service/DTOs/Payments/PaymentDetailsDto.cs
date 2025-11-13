using Eventify.APIs.DTOs.Bookings;

namespace Eventify.APIs.DTOs.Payments
{
    public class PaymentDetailsDto
    {
        public int BookingId { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public DateTime DateTime { get; set; }

        public BookingDto Booking { get; set; }

        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }

}
