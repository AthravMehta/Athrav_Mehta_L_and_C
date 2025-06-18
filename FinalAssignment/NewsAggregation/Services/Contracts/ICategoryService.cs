using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddAsync(CategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto dto);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }
}
