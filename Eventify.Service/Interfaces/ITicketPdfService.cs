using Eventify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ITicketPdfService
    {
        byte[] GenerateTicketPdf(Ticket ticket, Booking booking, Event eventEntity, string verificationUrl);
        byte[] GenerateAllTicketsPdf(List<Ticket> tickets, Booking booking, Event eventEntity, string verificationBaseUrl);
    }
}
