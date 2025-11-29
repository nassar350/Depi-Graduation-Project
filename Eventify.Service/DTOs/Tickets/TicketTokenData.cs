using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Tickets
{
    public class TicketTokenData
    {
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
