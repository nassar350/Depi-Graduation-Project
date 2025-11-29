using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventifyContext _context;
        public IBookingRepository _bookingRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IEventRepository _eventRepository { get; }
        public ITicketRepository _ticketRepository { get; }
        public IPaymentRepository _paymentRepository { get; }
        public IUserRepository _userRepository { get; }
        
        public UnitOfWork(EventifyContext context , IBookingRepository bookingRepository , ICategoryRepository categoryRepository, IEventRepository eventRepository , ITicketRepository ticketRepository , IPaymentRepository paymentRepository , IUserRepository userRepository)
        {
            _context = context;
            _bookingRepository = bookingRepository;
            _categoryRepository = categoryRepository;
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;   
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
