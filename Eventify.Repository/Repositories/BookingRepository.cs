using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eventify.Repository.Data.Contexts;

namespace Eventify.Repository.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly EventifyContext _context;

    public BookingRepository(EventifyContext context)
    {
        _context = context;
    }

    // Return all booking without their Users
    public async  Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .ToListAsync();
    }
    
    // Return Booking where Bookind ID == id  [Without User]

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        return booking;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


    public async Task<bool> UpdateAsync(int id, Booking booking)
    {
        var existing = await _context.Bookings.FindAsync(id);
        if (existing == null) return false;

        _context.Entry(existing).CurrentValues.SetValues(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Booking>> GetDetailedByUserId(int userId)
    {
        return await _context.Bookings
            .Include(b => b.Payment)
            .Include(b => b.Tickets)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }
}