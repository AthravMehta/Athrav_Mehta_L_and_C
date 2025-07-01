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

        public async Task<CategoryDto> AddCategoryAsync(string name)
        {
            var category = new CategoryDto { Name = name };
            CategoryDto response = await _apiService.PostAsync<CategoryDto>("/api/category", category);
            return response;
        }

        public async Task<List<CategoryDto>> GetAllCategoryAsync()
        {
            List<CategoryDto> categoryDtos = await _apiService.GetAsync<List<CategoryDto>>("/api/category");
            return categoryDtos;
        }

        public async Task<MessageResponseDto> HideCategoryAsync(int categoryId, string reason)
        {
            return await _apiService.PostAsync<MessageResponseDto>(
                $"/api/category/hide/{categoryId}", reason);
        }

        // Unhide category
        public async Task<MessageResponseDto> UnhideCategoryAsync(int categoryId)
        {
            return await _apiService.PostAsync<MessageResponseDto>(
                $"/api/category/unhide/{categoryId}", null);
        }
    }
}
