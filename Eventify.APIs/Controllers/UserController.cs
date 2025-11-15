using Eventify.Service.DTOs.Users;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateAsync(id, userDto);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }
    }
}
