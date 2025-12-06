using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Tickets
{
    public class TicketVerificationResponseDto
    {
        public bool IsValid { get; set; }
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public string AttendeeName { get; set; }
        public string Email { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string CategoryName { get; set; }
        public string EventStatus { get; set; } // "Upcoming", "Ongoing", "Ended"
        public string TicketStatus { get; set; } // "Valid", "Expired", "Cancelled", "Refunded", "PaymentRejected", "PaymentPending"
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string InvalidReason { get; set; } // Detailed reason why ticket is invalid
    }
}
