using Eventify.Service.DTOs.Payments;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var payments = await _paymentService.GetAllAsync();
                
                return Ok(new ApiResponseDto<IEnumerable<PaymentDto>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully",
                    Data = payments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving payments",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetById(int bookingId)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(bookingId);
                
                if (payment == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Payment for booking ID {bookingId} not found"
                    });
                }

                return Ok(new ApiResponseDto<PaymentDetailsDto>
                {
                    Success = true,
                    Message = "Payment details retrieved successfully",
                    Data = payment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the payment",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
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
                var created = await _paymentService.CreateAsync(dto);
                
                return CreatedAtAction(nameof(GetById), new { bookingId = created.BookingId },
                    new ApiResponseDto<PaymentDto>
                    {
                        Success = true,
                        Message = "Payment created successfully",
                        Data = created
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the payment",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{bookingId}")]
        [Authorize]
        public async Task<IActionResult> Update(int bookingId, [FromBody] UpdatePaymentDto dto)
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
                var updated = await _paymentService.UpdateAsync(bookingId, dto);
                
                if (updated == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Payment for booking ID {bookingId} not found"
                    });
                }

                return Ok(new ApiResponseDto<PaymentDto>
                {
                    Success = true,
                    Message = "Payment updated successfully",
                    Data = updated
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the payment",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{bookingId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int bookingId)
        {
            try
            {
                var deleted = await _paymentService.DeleteAsync(bookingId);
                
                if (!deleted)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = $"Payment for booking ID {bookingId} not found"
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Payment deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the payment",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("{bookingId}/refund")]
        [Authorize]
        public async Task<IActionResult> Refund(int bookingId)
        {
            try
            {
                var success = await _paymentService.RefundAsync(bookingId);

                if (!success)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Refund failed or payment not found",
                        Errors = new List<string> { "Unable to process refund for this booking" }
                    });
                }

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Payment refunded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while processing the refund",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
