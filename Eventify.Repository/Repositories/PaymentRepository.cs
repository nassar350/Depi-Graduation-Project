using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EventifyContext _context;

        public PaymentRepository(EventifyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int bookingId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetWithUserByIntentIdAsync(string paymentIntentId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);
        }
    }
}
