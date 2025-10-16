using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public int TicketsNum { get; set; }

        public int UserId { get; set; }
        public int EventId { get; set; }

        public Payment Payment { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
