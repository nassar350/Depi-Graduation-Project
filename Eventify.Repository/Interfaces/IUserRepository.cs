using Eventify.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Eventify.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);

        // Authentication-specific methods
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByPhoneNumberAsync(string phoneNumber);
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
    }
}
