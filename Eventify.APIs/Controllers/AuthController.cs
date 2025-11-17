using Eventify.API.Services.Auth;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Service.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenGenerator _jwt;

        public AuthController(UserManager<User> userManager, JwtTokenGenerator jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                PhoneNumber = dto.Phone,
                Role = Role.User
            };
            if (user.Name.Any(char.IsLetter) == false)
            {
                ModelState.AddModelError("Name", "Name must have at least one alphabet letter. ");
                return ValidationProblem(ModelState);
            }
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if(existingUser != null)
            {
                ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                return ValidationProblem(ModelState);
            }
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var check = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!check) return Unauthorized("Invalid email or password");

            var token = _jwt.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }
}
