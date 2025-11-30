using Eventify.Service.DTOs.Tickets;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketVerificationService _verificationService;

        public TicketsController(ITicketService ticketService, ITicketVerificationService verificationService)
        {
            _ticketService = ticketService;
            _verificationService = verificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ticketService.GetAllAsync();
            return Ok(new ApiResponseDto<IEnumerable<TicketDto>>
            {
                Success = true,
                Message = "Tickets retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _ticketService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"Ticket with ID {id} not found"
                });
            }

            return Ok(new ApiResponseDto<TicketDetailsDto>
            {
                Success = true,
                Message = "Ticket retrieved successfully",
                Data = result
            });
        }

        [HttpGet("available")]
        public IActionResult GetAvailableTicketsCount([FromQuery] int eventId, [FromQuery] string categoryName)
        {
            if (eventId <= 0)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid event ID",
                    Errors = new List<string> { "Event ID must be a positive number" }
                });
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid category name",
                    Errors = new List<string> { "Category name is required" }
                });
            }

            try
            {
                var availableCount = _ticketService.GetAvailableTicketsCount(eventId, categoryName);

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Available tickets count retrieved successfully",
                    Data = new
                    {
                        eventId = eventId,
                        categoryName = categoryName,
                        availableTickets = availableCount
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving available tickets count",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
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

            try
            {
                var created = await _ticketService.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.ID },
                    new ApiResponseDto<TicketDto>
                    {
                        Success = true,
                        Message = "Ticket created successfully",
                        Data = created
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the ticket",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
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

            try
            {
                var success = await _ticketService.UpdateAsync(id, dto);
                if (!success)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Ticket with ID {id} not found"
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Ticket updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the ticket",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _ticketService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Ticket with ID {id} not found"
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Ticket deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the ticket",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Verify if a ticket is valid (event hasn't ended)
        /// </summary>
        /// <param name="token">Encrypted ticket token from QR code</param>
        /// <returns>Validation result with ticket details</returns>
        [HttpGet("verify/{token}")]
        public async Task<IActionResult> VerifyTicket(string token)
        {
            var (success, message, data, errors) = await _verificationService.VerifyTicketAsync(token);

            var statusCode = success ? 200 : 400;

            return StatusCode(statusCode, new
            {
                success,
                message,
                data,
                errors
            });
        }
    }
}
