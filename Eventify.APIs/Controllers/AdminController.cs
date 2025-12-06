using Eventify.Service.DTOs.Admin;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region User Management

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] string searchTerm = null)
        {
            var result = await _adminService.GetAllUsersAsync(searchTerm);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.UpdateUserAsync(id, dto);
            if (!result.Success)
            {
                if (result.Errors.Any(e => e.Field == "NotFound"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result.Success)
            {
                if (result.Errors.Any(e => e.Field == "NotFound"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Event Management

        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvents([FromQuery] string searchTerm = null)
        {
            var result = await _adminService.GetAllEventsAsync(searchTerm);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _adminService.DeleteEventAsync(id);
            if (!result.Success)
            {
                if (result.Errors.Any(e => e.Field == "NotFound"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Booking Management

        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings([FromQuery] string searchTerm = null)
        {
            var result = await _adminService.GetAllBookingsAsync(searchTerm);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("bookings/{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _adminService.CancelBookingAsync(id);
            if (!result.Success)
            {
                if (result.Errors.Any(e => e.Field == "NotFound"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Payment Management

        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPayments([FromQuery] string searchTerm = null)
        {
            var result = await _adminService.GetAllPaymentsAsync(searchTerm);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Event Category Management

        [HttpGet("eventcategories")]
        public async Task<IActionResult> GetAllEventCategories()
        {
            var result = await _adminService.GetAllEventCategoriesAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion

        #region Statistics

        [HttpGet("statistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var result = await _adminService.GetDashboardStatisticsAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        #endregion
    }
}
