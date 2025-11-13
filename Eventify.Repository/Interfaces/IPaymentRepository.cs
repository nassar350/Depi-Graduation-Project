using Eventify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int bookingId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(Payment payment);
        Task SaveChangesAsync();
        Task<Payment?> GetWithUserByIntentIdAsync(string paymentIntentId);
    }
}
