using Eventify.Service.DTOs.Auth;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutService _checkoutService;

        public CheckOutController(ICheckOutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CheckOutRequestDto dto)
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
                var result = await _checkoutService.CreateCheckoutAsync(dto);
                if (result.Success)
                {

                    return Ok(new ApiResponseDto<CheckoutResponseDto>
                    {
                        Success = true,
                        Message = "Checkout session created successfully",
                        Data = result.Data
                    });
                }
                else
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = result.Errors.Select(e => e.Message).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "An error occurred during checkout. Please try again later.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
