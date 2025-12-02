using Eventify.Service.DTOs.Users;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.Helpers;

namespace Eventify.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
            if (!result.Success)
                return BadRequest(ModelState);

            return Ok(result.Data);
        }
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
            if (!result.Success)
                return BadRequest(ModelState);
            return Ok(result.Data);
        }
        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var CurrentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (CurrentUserID == null)
            {
                return Unauthorized();
            }
            var result = await _userService.GetByIdAsync(int.Parse(CurrentUserID));
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
            if (!result.Success)
                return BadRequest(ModelState);
            return Ok(result.Data);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var result = await _userService.UpdateAsync(id, userDto);

            if (!result.Success)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Failed to update user",
                    Errors = result.Errors.Select(e => e.Message).ToList()
                });
            }

            return Ok(new ApiResponseDto<UserUpdateDto>
            {
                Success = true,
                Message = "User updated successfully",
                Data = result.Data
            });
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
            if (!result.Success)
                return BadRequest(ModelState);

            return NoContent();
        }

        [HttpGet("booked")]
        public IActionResult GetTicketsBookedCount()
        {
            var CurrentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(CurrentUserID, out int Id))
            {
                return BadRequest("Invalid claim ID");
            }            
            if (Id <= 0)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid User ID",
                    Errors = new List<string> { "User ID must be a positive number" }
                });
            }

            
            var result = _userService.GetTicketsBookedCount(Id);
            return Ok(new ApiResponseDto<int>
            {
                Success = true,
                Message = "Total tickets fetched successfully",
                Data = result
            });
            
        }
        [HttpGet("revenue")]
        public IActionResult GetUserTotalRevenue()
        {
            var CurrentUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(CurrentUserID, out int Id))
            {
                return BadRequest("Invalid claim ID");
            }            
            if (Id <= 0)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid User ID",
                    Errors = new List<string> { "User ID must be a positive number" }
                });
            }

            
            var result = _userService.GetTotalRevenueById(Id);
            return Ok(new ApiResponseDto<ServiceResult<decimal>>
            {
                Success = true,
                Message = "Total revenue fetched successfully",
                Data = result
            });
            
        }
    }
}
