using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IBookingRepository _bookingRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IEventRepository _eventRepository { get; }
        public ITicketRepository _ticketRepository { get; }
        public IPaymentRepository _paymentRepository { get; }
        public IUserRepository _userRepository { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
