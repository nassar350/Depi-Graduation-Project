using AutoMapper;
using Eventify.API.Services.Auth;
using Eventify.Core.Entities;
using Eventify.Core.Enums;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Auth;
using Eventify.Service.DTOs.Users;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Eventify.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResult<UserInfoDto>> RegisterAsync(UserRegisterDto dto)
        {
            try
            {
                var errors = new List<ValidationError>();

                if (!dto.Name.Any(char.IsLetter))
                {
                    errors.Add(new ValidationError { Field = "Name", Message = "Name must have at least one alphabet letter." });
                }

                var existingUserByEmail = await _userRepository.FindByEmailAsync(dto.Email);
                if (existingUserByEmail != null)
                {
                    errors.Add(new ValidationError { Field = "Email", Message = "This email is already registered." });
                }

                var existingUserByPhone = await _userRepository.FindByPhoneNumberAsync(dto.Phone);
                if (existingUserByPhone != null)
                {
                    errors.Add(new ValidationError { Field = "Phone", Message = "This phone number is already registered." });
                }

                if (errors.Any())
                {
                    return ServiceResult<UserInfoDto>.Fail(errors);
                }

                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name,
                    PhoneNumber = dto.Phone,
                    Role = Role.User
                };

                var result = await _userRepository.CreateUserAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var identityErrors = result.Errors.Select(e => new ValidationError 
                    { 
                        Field = e.Code, 
                        Message = e.Description 
                    }).ToList();
                    
                    return ServiceResult<UserInfoDto>.Fail(identityErrors);
                }

                var userInfo = new UserInfoDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString()
                };

                return ServiceResult<UserInfoDto>.Ok(userInfo);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserInfoDto>.Fail("Exception", $"An error occurred during registration: {ex.Message}");
            }
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ServiceResult<AuthResponseDto>.Fail("Login", "Invalid email or password");
                }

                var passwordCheck = await _userRepository.CheckPasswordAsync(user, dto.Password);
                if (!passwordCheck)
                {
                    return ServiceResult<AuthResponseDto>.Fail("Login", "Invalid email or password");
                }

                var token = _jwtTokenGenerator.GenerateToken(user);
                var jwtSettings = _configuration.GetSection("Jwt");
                var durationInMinutes = double.Parse(jwtSettings["DurationInMinutes"]);

                var authResponse = new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString()
                    },
                    ExpiresAt = DateTime.UtcNow.AddMinutes(durationInMinutes)
                };

                return ServiceResult<AuthResponseDto>.Ok(authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponseDto>.Fail("Exception", $"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> LogoutAsync()
        {
            try
            {
                return ServiceResult<string>.Ok("Logout successful. Please remove the token from client storage.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail("Exception", $"An error occurred during logout: {ex.Message}");
            }
        }
    }
}