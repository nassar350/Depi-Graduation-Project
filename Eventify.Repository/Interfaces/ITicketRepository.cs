using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eventify.Core.Entities;

namespace Eventify.Repository.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        int CountNotBookedTickets(int eventid , string catName);
        Task<IEnumerable<Ticket>> GetNotBookedTickets(int eventid , string catName , int q);
        Task<Ticket?> GetByIdAsync(int id);
        Task AddAsync(Ticket ticket);
        Task<IEnumerable<Ticket>> AddRangeAsync(IEnumerable<Ticket> tickets);
        void UpdateRange(IEnumerable<Ticket> tickets);
        void Update(Ticket ticket);
        void Delete(Ticket ticket);
        int CountBookedTickets(int eventId);
    }
}
