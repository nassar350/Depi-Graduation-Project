using Eventify.Service.DTOs.Auth;
using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Interfaces;
using Eventify.Service.Services;
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
        private readonly ITicketDownloadService _ticketDownloadService;

        public CheckOutController(ICheckOutService checkoutService, ITicketDownloadService ticketDownloadService)
        {
            _checkoutService = checkoutService;
            _ticketDownloadService = ticketDownloadService;
        }

        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Download all tickets for a booking as PDF
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <returns>PDF file with all tickets (one page per ticket)</returns>
        [HttpGet("tickets/{bookingId}/pdf")]
        public async Task<IActionResult> DownloadTicketPdf(int bookingId)
        {
            var (success, message, data, errors) = await _ticketDownloadService.GenerateTicketsPdfAsync(bookingId);

            if (!success)
            {
                var statusCode = message.Contains("not found") ? 404 : 500;
                return StatusCode(statusCode, new
                {
                    success,
                    message,
                    data = (object)null,
                    errors
                });
            }

            return File(data.PdfBytes, "application/pdf", data.FileName);
        }
    }
}
