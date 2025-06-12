using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddAsync(CategoryDto dto);
        Task<CategoryDto> UpdateAsync(Guid id, CategoryDto dto);
        Task DeleteAsync(Guid id);
        Task<CategoryDto> GetByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
    }
}
