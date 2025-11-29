using Eventify.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Eventify.Service.DTOs.Payments
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Booking ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Booking ID must be a positive number")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Total price must be between 0.01 and 1,000,000")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Payment method must be between 2 and 50 characters")]
        [RegularExpression(
             @"^(Credit Card|Debit Card|PayPal|Apple Pay|Google Pay|Bank Transfer|Cash)$",
             ErrorMessage = "Invalid payment method"
         )]
        public string PaymentMethod { get; set; }= string.Empty;

        [Required(ErrorMessage = "Payment status is required")]
        [EnumDataType(typeof(PaymentStatus), ErrorMessage = "Invalid payment status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        //seeeeeeeeeeeeeeeeeeeeeeeee
        [DataType(DataType.DateTime)]
        public DateTime? DateTime { get; set; }= System.DateTime.UtcNow;
    }

}
