using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Bookings
{
    public class CheckoutResponseDto
    {
        public int BookingId { get; set; }
        public int PaymentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
