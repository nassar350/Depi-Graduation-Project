using Eventify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public TicketStatus Status { get; set; }
        public int TicketsNum { get; set; }
        public int UserId { get; set; }

        public Payment Payment { get; set; }
        public User User { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
