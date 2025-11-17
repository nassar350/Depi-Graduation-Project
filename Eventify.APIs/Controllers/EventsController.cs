using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
namespace Eventify.APIs.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ICategoryService _categoryService;
    private readonly ITicketService _ticketService;

    public EventsController(IEventService eventService , ICategoryService categoryService , ITicketService ticketService)
    {
        _eventService = eventService;
        _categoryService = categoryService;
        _ticketService = ticketService;
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
    [Authorize(Roles = "Admin,User")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userID == null)
        {
            return Unauthorized();
        }

        var created = await _eventService.CreateAsync(dto , userID);
        if (!created.Success)
        {
            foreach (var error in created.Errors)
                ModelState.AddModelError(error.Field, error.Message);

            return BadRequest(ModelState);
        }
        return CreatedAtAction(nameof(Get), new { id = created.Data.Id }, created);
    }

    //[Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateEventDto dto)
    {
        var ok = await _eventService.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    //[Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _eventService.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
