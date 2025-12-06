using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Eventify.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

public class EventRepository : IEventRepository
{
    private readonly EventifyContext _context;
    public EventRepository(EventifyContext context) => _context = context;

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Tickets)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Event> GetByIdAsync(int id)
    {
        var entity = await _context.Events
            .Include(e => e.Organizer)
            .FirstOrDefaultAsync(e => e.Id == id);
        return entity ?? throw new KeyNotFoundException($"Event with id {id} not found.");
    }

    public async Task<Event> AddAsync(Event entity)
    {
        await _context.Events.AddAsync(entity);
        return entity;
    }

    public void Update(Event entity)
    {
        _context.Events.Update(entity);
    }

    public void Delete(Event entity)
    {
        _context.Events.Remove(entity);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Events.AnyAsync(e => e.Id == id);
    }

    public async Task<Event?> GetEventWithDetailsAsync(int id)
    {
        return await _context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Categories)
            .Include(e => e.Tickets)
            .Include(e => e.EventsAttendedByUsers)
                .ThenInclude(ua => ua.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetUpcommingAsync(int take)
    {
        return await _context.Events
            .Where(e => e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .Take(take)
            .Include(e => e.Organizer)
            .Include(e => e.Categories)
            .AsNoTracking()
            .ToListAsync();
    }
}
