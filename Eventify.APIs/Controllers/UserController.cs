using Eventify.Service.DTOs.Users;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                return BadRequest(ModelState);

            var result = await _userService.UpdateAsync(id, userDto);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
            if (!result.Success)
                return BadRequest(ModelState);
            return Ok(result.Data);
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
    }
}
