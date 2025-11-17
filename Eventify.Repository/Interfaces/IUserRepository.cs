using Eventify.Core.Entities;

namespace Eventify.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id , User user);
        Task<bool> DeleteAsync(int id);

    }
}
