using Eventify.Service.DTOs.Bookings;

namespace Eventify.Service.DTOs.Payments
{
    public class PaymentDetailsDto
    {
        public int BookingId { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public DateTime DateTime { get; set; }

        public BookingDto Booking { get; set; }
    }

}
