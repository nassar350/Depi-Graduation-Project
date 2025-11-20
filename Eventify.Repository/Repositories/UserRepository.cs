using Eventify.Core.Entities;
using Eventify.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id.ToString());
            if (userToDelete == null) return false;
            await _userManager.DeleteAsync(userToDelete);
            return true;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var userToFind = await _userManager.FindByIdAsync(id.ToString());
            return userToFind;
        }

        public async Task<User> GetUserByEmail(String email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            var userToUpdate = await _userManager.FindByIdAsync(id.ToString());
            if (userToUpdate == null) return false;
            await _userManager.UpdateAsync(user);
            return true;
        }
    }
}
