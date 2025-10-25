public class EventRepository : IEventRepository
{
    private readonly EventifyContext _context;
    public EventRepository(EventifyContext context) => _context = context;

    public async Task<IEnumerable<Event>> GetAllAsync()
        => await _context.Events.AsNoTracking().ToListAsync();

    public async Task<Event> GetByIdAsync(int id)
        => await _context.Events
                         .Include(e => e.TicketTypes) // لو مرتبط
                         .FirstOrDefaultAsync(e => e.EventID == id);

    public async Task AddAsync(Event entity)
    {
        await _context.Events.AddAsync(entity);
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
        => await _context.Events.AnyAsync(e => e.EventID == id);
}
