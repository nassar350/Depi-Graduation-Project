using Eventify.Core.Entities;

namespace Eventify.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task<bool> UpdateAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);
    }
}

