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
        public async Task<IEnumerable<Ticket>> AddRangeAsync(IEnumerable<Ticket> tickets)
        {
            await _context.Tickets.AddRangeAsync(tickets);
            return tickets;
        }
        public void Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public void Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }

        public int CountNotBookedTickets(int eventid , string catName)
        {
            int CatID = _context.Categories.Where(c => c.EventId == eventid && c.Title == catName).Select(c => c.Id).FirstOrDefault();
            return _context.Tickets.Count(t => t.BookingId == null && t.CategoryId == CatID);
        }


        public async Task<IEnumerable<Ticket>> GetNotBookedTickets(int eventid, string catName , int q)
        {
            int CatID = _context.Categories.Where(c => c.EventId == eventid && c.Title == catName).Select(c => c.Id).FirstOrDefault();
            return await _context.Tickets.Where(t => t.BookingId == null && t.CategoryId == CatID).OrderBy(t => t.ID).Take(q).ToListAsync();
        }

        public  void UpdateRange(IEnumerable<Ticket> tickets) =>  _context.Tickets.UpdateRange(tickets);
    }
}
