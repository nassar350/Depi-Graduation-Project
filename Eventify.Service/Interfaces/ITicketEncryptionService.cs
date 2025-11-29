using Eventify.Service.DTOs.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ITicketEncryptionService
    {
        string GenerateTicketToken(int ticketId, int bookingId);
        TicketTokenData DecryptToken(string encryptedToken);
    }
}
