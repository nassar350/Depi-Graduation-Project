using Eventify.Service.DTOs.Bookings;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create([FromBody] CheckOutRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _checkoutService.CreateCheckoutAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during checkout. Please try again later.");
            }
        }
    }
}
