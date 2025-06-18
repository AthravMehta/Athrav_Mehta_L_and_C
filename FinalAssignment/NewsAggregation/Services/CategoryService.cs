using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICrudBaseRepository<Category> _categoryRepo;

        public CategoryService(ICrudBaseRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CategoryDto> AddAsync(CategoryDto dto)
        {
            var category = new Category { Name = dto.Name };
            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
            return new CategoryDto { Id = category.CategoryId, Name = category.Name };
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return null;
            category.Name = dto.Name;
            await _categoryRepo.UpdateAsync(category);
            await _categoryRepo.SaveChangesAsync();
            return new CategoryDto { Id = category.CategoryId, Name = category.Name };
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.CategoryId, Name = c.Name });
        }
    }
}
