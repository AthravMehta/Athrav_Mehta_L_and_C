using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddAsync(CategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto dto);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<int> GetCategoryIdAsync(Article article, List<Keywords>? keywords, List<CategoryDto>? categories);
        Task<bool> HideCategoryAsync(int categoryId, string reason);
        Task<bool> UnhideCategoryAsync(int categoryId);

    }
}
