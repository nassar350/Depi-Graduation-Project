using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Eventify.APIs.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ICategoryService _categoryService;

    public EventsController(IEventService eventService , ICategoryService categoryService)
    {
        _eventService = eventService;
        _categoryService = categoryService;
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
        if (dto.EndDate <= dto.StartDate)
        {
            ModelState.AddModelError("EndDate", "End Date must be after the Start Date");
            return BadRequest(ModelState);  
        }
        if(dto.StartDate == DateTime.UtcNow)
        {
            ModelState.AddModelError("StartDate", "Start Date must be in the future");
        }
        var Categories = JsonConvert.DeserializeObject<List<CategoryCreationByEventDto>>(dto.CategoriesJson);

        var created = await _eventService.CreateAsync(dto);

        if(Categories != null)
        foreach (var category in Categories)
        {
            await _categoryService.CreateAsync(new CreateCategoryDto(created.Id, category.Title, category.Seats));
        }
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
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
