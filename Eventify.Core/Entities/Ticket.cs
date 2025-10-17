using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Core.Entities
{
    public class Ticket
    {
        public int ID { get; set; }

        public string Place { get; set; }
        public string Type { get; set; }

        public int? BookingId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }

        public Event Event { get; set; }
        public Booking Booking { get; set; }

        public Category Category { get; set; }
    }

}
