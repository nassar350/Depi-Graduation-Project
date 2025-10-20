﻿using Eventify.Core.Enums;

namespace Eventify.APIs.DTOs.Payments
{
    public class UpdatePaymentDto
    {
        public decimal? TotalPrice { get; set; }

        public string? PaymentMethod { get; set; }

        public PaymentStatus? Status { get; set; }

        public DateTime? DateTime { get; set; }
    }

}
