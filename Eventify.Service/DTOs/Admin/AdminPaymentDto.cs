using Eventify.Core.Enums;

namespace Eventify.Service.DTOs.Admin
{
    public class AdminPaymentDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string UserName { get; set; }
        public string EventName { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        public string StripePaymentIntentId { get; set; }
    }
}
