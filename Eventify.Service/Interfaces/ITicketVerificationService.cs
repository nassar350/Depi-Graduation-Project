using Eventify.Service.DTOs.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ITicketVerificationService
    {
        Task<(bool Success, string Message, TicketVerificationResponseDto Data, string[] Errors)> VerifyTicketAsync(string token);
    }
}
