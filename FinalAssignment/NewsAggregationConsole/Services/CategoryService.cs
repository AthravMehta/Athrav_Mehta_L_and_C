using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Services
{
    public class CategoryService
    {
        private readonly ApiService _apiService;

        public CategoryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task AddCategoryAsync(string name)
        {
            var category = new CategoryDto { Name = name };
            await _apiService.PostAsync<CategoryDto>("/api/category", category);
        }

        public async Task<List<CategoryDto>> GetAllCategoryAsync()
        {
            List<CategoryDto> categoryDtos = await _apiService.GetAsync<List<CategoryDto>>("/api/category");
            return categoryDtos;
        }
    }
}
