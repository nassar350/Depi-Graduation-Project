using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Repository.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly EventifyContext _context;

        public TicketRepository(EventifyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.Category)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                    .Include(t => t.Event)
                    .FirstOrDefaultAsync(t => t.ID == id);
        }

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }
    }
}
