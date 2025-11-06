using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Core.Entities;
using Eventify.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EventifyContext _context;

        public CategoryRepository(EventifyContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            existing.Title = category.Title;
            existing.Seats = category.Seats;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
