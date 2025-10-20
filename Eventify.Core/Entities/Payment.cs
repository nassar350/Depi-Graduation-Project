using Eventify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Eventify.Core.Entities
{
    public class Payment
    {
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        public Booking Booking { get; set; }
    }
}
