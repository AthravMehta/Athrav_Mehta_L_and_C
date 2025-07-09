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

        /// <summary>
        /// Hide any Category, only Admin can do
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<bool> HideCategoryAsync(int categoryId, string reason);

        /// <summary>
        /// Unhide Any hidden Category, only Admin can do
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<bool> UnhideCategoryAsync(int categoryId);

    }
}
