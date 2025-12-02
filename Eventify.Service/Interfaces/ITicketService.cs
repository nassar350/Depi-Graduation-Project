using Eventify.Service.DTOs.Tickets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllAsync();
        Task<TicketDetailsDto?> GetByIdAsync(int id);
        Task<TicketDto> CreateAsync(CreateTicketDto dto);
        Task<bool> UpdateAsync(int id, UpdateTicketDto dto);
        Task<bool> DeleteAsync(int id);
        int GetAvailableTicketsCount(int eventId, string categoryName);
        int GetBookedTicketsCount(int eventId);
    }
}
