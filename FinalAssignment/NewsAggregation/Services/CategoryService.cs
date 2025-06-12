using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICrudBaseRepository<Category, Guid> _categoryRepo;

        public CategoryService(ICrudBaseRepository<Category, Guid> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CategoryDto> AddAsync(CategoryDto dto)
        {
            var category = new Category { Name = dto.Name };
            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, CategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return null;
            category.Name = dto.Name;
            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category != null)
            {
                _categoryRepo.Delete(category);
                await _categoryRepo.SaveChangesAsync();
            }
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            return category == null ? null : new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
        }
    }
}
