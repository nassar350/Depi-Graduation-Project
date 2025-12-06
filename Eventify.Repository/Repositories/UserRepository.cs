using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        
        private readonly EventifyContext _context;


        public UserRepository(UserManager<User> userManager , EventifyContext context )
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id.ToString());
            if (userToDelete == null) return false;
            await _userManager.DeleteAsync(userToDelete);
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var userToFind = await _userManager.FindByIdAsync(id.ToString());
            return userToFind;
        }

        public async Task<User> GetUserByEmail(String email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            var userToUpdate = await _userManager.FindByIdAsync(id.ToString());
            if (userToUpdate == null) return false;
            await _userManager.UpdateAsync(user);
            return true;
        }

        // Authentication-specific methods
        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        
       
        public int CountTicketBooked(int userId )
        {
            int totalTickets = _context.Bookings
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId 
                    && b.Status == BookingStatus.Booked 
                    && (b.Payment == null || b.Payment.Status == PaymentStatus.Paid))
                .Sum(b => (int?)b.TicketsNum) ?? 0;

            return totalTickets;
        }

        public decimal GetTotalRevenueById(int id)
        {
            var totalRevenue = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Tickets)
                        .ThenInclude(t => t.Event)
                .Where(p => p.Booking.Tickets.Any(t => t.Event.OrganizerID == id))
                .Sum(p => (decimal?)p.TotalPrice) ?? 0;

            return totalRevenue;
        }
    }
}
