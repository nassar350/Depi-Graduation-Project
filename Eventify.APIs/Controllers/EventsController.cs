using Eventify.Service.DTOs.Categories;
using Eventify.Service.DTOs.Events;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Eventify.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _eventService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<EventDto>>
            {
                Success = true,
                Message = "Events retrieved successfully",
                Data = result
            });
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] int take = 10)
        {
            var result = await _eventService.GetUpcomingEventsAsync(take);

            if (!result.Success)
            {
                return Ok(new ApiResponseDto<IEnumerable<EventDto>>
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? "No upcoming events found",
                    Data = Enumerable.Empty<EventDto>(),
                    Errors = result.Errors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new ApiResponseDto<IEnumerable<EventDto>>
            {
                Success = true,
                Message = "Upcoming events retrieved successfully",
                Data = result.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _eventService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? $"Event with ID {id} not found",
                    Errors = result.Errors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new ApiResponseDto<EventDetailsDto>
            {
                Success = true,
                Message = "Event details retrieved successfully",
                Data = result.Data
            });
        }

        [HttpGet("organizer/{organizerId}")]
        public async Task<IActionResult> GetEventsByOrganizer(int organizerId)
        {
            var result = await _eventService.GetEventsByOrganizerAsync(organizerId);

            if (!result.Success)
            {
                return Ok(new ApiResponseDto<IEnumerable<EventDto>>
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? "No events found for this organizer",
                    Data = Enumerable.Empty<EventDto>(),
                    Errors = result.Errors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new ApiResponseDto<IEnumerable<EventDto>>
            {
                Success = true,
                Message = "Organizer events retrieved successfully",
                Data = result.Data
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEvents([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Search term is required",
                    Errors = new List<string> { "Search term cannot be empty" }
                });
            }

            var result = await _eventService.SearchEventsAsync(searchTerm);

            if (!result.Success)
            {
                return Ok(new ApiResponseDto<IEnumerable<EventDto>>
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? "No events found matching your search",
                    Data = Enumerable.Empty<EventDto>(),
                    Errors = result.Errors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new ApiResponseDto<IEnumerable<EventDto>>
            {
                Success = true,
                Message = $"Found {result.Data?.Count() ?? 0} events matching your search",
                Data = result.Data
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userID == null)
            {
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "User authentication required"
                });
            }

            var created = await _eventService.CreateAsync(dto, userID);
            if (!created.Success)
            {
                var errorMessages = created.Errors.Select(e => e.Message).ToList();
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Event creation failed",
                    Errors = errorMessages
                });
            }

            return CreatedAtAction(nameof(Get), new { id = created.Data.Id }, new ApiResponseDto<EventDto>
            {
                Success = true,
                Message = "Event created successfully",
                Data = created.Data
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var ok = await _eventService.UpdateAsync(id, dto);
            if (!ok)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"Event with ID {id} not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Event updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _eventService.DeleteAsync(id);
            if (!ok)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"Event with ID {id} not found"
                });
            }

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Event deleted successfully"
            });
        }
    }
}
