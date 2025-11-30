using Eventify.Service.DTOs.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ITicketDownloadService
    {
        Task<(bool Success, string Message, TicketDownloadResponseDto Data, string[] Errors)> GenerateTicketsPdfAsync(int bookingId);
    }
}
