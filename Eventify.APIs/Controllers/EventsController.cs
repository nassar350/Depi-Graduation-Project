[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EventsController(IEventService eventService, IHttpContextAccessor httpContextAccessor)
    {
        _eventService = eventService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _eventService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _eventService.GetByIdAsync(id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        int organizerId = 1;
        var created = await _eventService.CreateAsync(dto, organizerId);
        return CreatedAtAction(nameof(Get), new { id = created.EventID }, created);
    }

    //[Authorize(Roles = "Organizer,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
    {
        var ok = await _eventService.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    //[Authorize(Roles = "Organizer,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _eventService.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
