using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _service.GetAllAsync();
            
            if (!bookings.Any())
            {
                return Ok(new ApiResponseDto<IEnumerable<BookingDto>>
                {
                    Success = false,
                    Message = "No bookings found",
                    Data = Enumerable.Empty<BookingDto>()
                });
            }

            return Ok(new ApiResponseDto<IEnumerable<BookingDto>>
            {
                Success = true,
                Message = "Bookings retrieved successfully",
                Data = bookings
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetByIdAsync(id);
            
            if (booking == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"Booking with ID {id} not found"
                });
            }

            return Ok(new ApiResponseDto<BookingDto>
            {
                Success = true,
                Message = "Booking retrieved successfully",
                Data = booking
            });
        }
        [HttpGet("User")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUserId()
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _service.GetByUserId(int.Parse(currentId));
            if (!bookings.Data.Any())
            {
                return Ok(new ApiResponseDto<IEnumerable<BookingDetailsDto>>
                {
                    Success = false,
                    Message = "No bookings found",
                    Data = Enumerable.Empty<BookingDetailsDto>()
                });
            }

            return Ok(new ApiResponseDto<IEnumerable<BookingDetailsDto>>
            {
                Success = true,
                Message = "Bookings retrieved successfully",
                Data = bookings.Data
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
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
                var created = await _service.CreateAsync(dto);
                
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, 
                    new ApiResponseDto<BookingDto>
                    {
                        Success = true,
                        Message = "Booking created successfully",
                        Data = created
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the booking",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto)
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
                var success = await _service.UpdateAsync(id, dto);
                
                if (!success)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Booking with ID {id} not found"
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Booking updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the booking",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                
                if (!success)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Booking with ID {id} not found"
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Booking deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the booking",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
