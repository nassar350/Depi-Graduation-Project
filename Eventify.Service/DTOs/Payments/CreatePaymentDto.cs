using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Payments
{
    public class CreatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime? DateTime { get; set; }
    }

}
