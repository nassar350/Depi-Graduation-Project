using Eventify.Service.DTOs.Bookings;

namespace Eventify.Service.DTOs.Payments
{
    public class PaymentDetailsDto
    {
        public int BookingId { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }= string.Empty;
        public string? StripePaymentIntentId { get; set; }
        public string? StripeClientSecret { get; set; }

        public string Status { get; set; }= string.Empty;

        public DateTime DateTime { get; set; }

        public BookingDto Booking { get; set; }= new();

        public string CustomerEmail { get; set; }= string.Empty;
        public string CustomerPhone { get; set; }= string.Empty;
    }

}
