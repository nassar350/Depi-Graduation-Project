using AutoMapper;
using Eventify.Service.DTOs.Categories;
using Eventify.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventify.Repository.Interfaces;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            var created = await _repo.AddAsync(category);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var updatedCategory = _mapper.Map<Category>(dto);
            return await _repo.UpdateAsync(id, updatedCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
