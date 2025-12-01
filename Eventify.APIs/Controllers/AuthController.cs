using Eventify.Service.DTOs.Users;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
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

            var result = await _authService.RegisterAsync(dto);
            
            if (!result.Success)
            {
                var errorMessages = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? "Registration failed",
                    Errors = errorMessages
                });
            }

            return Ok(new ApiResponseDto<UserInfoDto>
            {
                Success = true,
                Message = "User registered successfully",
                Data = result.Data
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
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

            var result = await _authService.LoginAsync(dto);
            
            if (!result.Success)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Message ?? "Login failed"
                });
            }

            return Ok(result.Data);
        }
    }
}
