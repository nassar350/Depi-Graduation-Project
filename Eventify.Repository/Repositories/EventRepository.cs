using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;


public class EventRepository : IEventRepository
{
    private readonly EventifyContext _context;
    public EventRepository(EventifyContext context) => _context = context;

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _context.Events.AsNoTracking().ToListAsync();

    public async Task<Event> GetByIdAsync(int id)
    {
                          var entity = await _context.Events
                             .FirstOrDefaultAsync(e => e.Id == id);
        return entity?? throw new KeyNotFoundException($"Event with id {id} not found.");
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
        return await _context.Events.AnyAsync(e =>e.Id == id);
    }
}
