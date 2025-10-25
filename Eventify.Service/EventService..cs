public class EventService : IEventService
{
    private readonly IEventRepository _repo;
    private readonly IMapper _mapper;
    private readonly EventifyContext _context; 

    public EventService(IEventRepository repo, IMapper mapper, EventifyContext context)
    {
        _repo = repo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<EventDto>> GetAllAsync()
    {
        var events = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<EventDto>>(events);
    }

    public async Task<EventDto> GetByIdAsync(int id)
    {
        var ev = await _repo.GetByIdAsync(id);
        return _mapper.Map<EventDto>(ev);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        var ev = _mapper.Map<Event>(dto);
        await _repo.AddAsync(ev);
        await _context.SaveChangesAsync();
        return _mapper.Map<EventDto>(ev);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEventDto dto)
    {
        var ev = await _repo.GetByIdAsync(id);
        if (ev == null) return false;
        _mapper.Map(dto, ev);
        _repo.Update(ev);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ev = await _repo.GetByIdAsync(id);
        if (ev == null) return false;
        _repo.Delete(ev);
        await _context.SaveChangesAsync();
        return true;
    }
}
