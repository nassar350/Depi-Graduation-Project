namespace Eventify.Service.DTOs.Payments
{
    public class PaymentDto
    {
        public int BookingId { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }= string.Empty;
        public string? StripePaymentIntentId { get; set; }
        public string? StripeClientSecret { get; set; }
        public string Status { get; set; }= string.Empty;

        public DateTime DateTime { get; set; }
    }

}
