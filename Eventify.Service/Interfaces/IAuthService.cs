using Eventify.Service.DTOs.Auth;
using Eventify.Service.DTOs.Users;
using Eventify.Service.Helpers;

namespace Eventify.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<UserInfoDto>> RegisterAsync(UserRegisterDto dto);
        Task<ServiceResult<AuthResponseDto>> LoginAsync(UserLoginDto dto);
        Task<ServiceResult<string>> LogoutAsync();
    }
}