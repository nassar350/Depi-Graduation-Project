using AutoMapper;
using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Eventify.Service.DTOs.Users;
using Eventify.Service.Helpers;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository , IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var result = await _userRepository.DeleteAsync(id);

            if (!result)
                return ServiceResult<bool>.Fail($"Failed to delete user with ID '{id}'.");

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var mappedUsers = _mapper.Map<IEnumerable<UserDto>>(users);
            if (users == null || !users.Any())
            {
                return ServiceResult<IEnumerable<UserDto>>.Fail("No users found.");
            }

            return ServiceResult<IEnumerable<UserDto>>.Ok(mappedUsers);
        }

        public async Task<ServiceResult<UserDto>> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            var mappedUser = _mapper.Map<UserDto>(user);
            if(user == null)
            {
                return ServiceResult<UserDto>.Fail($"No user found with id {id}");
            }
            return ServiceResult<UserDto>.Ok(mappedUser);
        }

        public async Task<ServiceResult<UserUpdateDto>> UpdateAsync(int id, UserUpdateDto user)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return ServiceResult<UserUpdateDto>.Fail($"User with ID '{id}' not found.");
            existingUser.Name = user.Name;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Email = user.Email;

            var updateResult = await _userRepository.UpdateAsync(id, existingUser);
            if (!updateResult)
                return ServiceResult<UserUpdateDto>.Fail($"Failed to update user with ID '{id}'.");

            return ServiceResult<UserUpdateDto>.Ok(user);
        }
    }
}
