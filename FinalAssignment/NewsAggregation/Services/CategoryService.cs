using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IKeywordService _keywordService;
        private readonly ICrudBaseRepository<User> _userRepository;
        private readonly ICrudBaseRepository<Category> _categoryRepository;
        private readonly IUserNotificationConfigurationService _userNotificationConfigurationService;

        public CategoryService(
            ICrudBaseRepository<Category> categoryRepo,
            IKeywordService keywordService,
            IUserNotificationConfigurationService userNotificationConfigurationService)
        {
            _categoryRepository = categoryRepo;
            _keywordService = keywordService;
            _userNotificationConfigurationService = userNotificationConfigurationService;
        }

        public async Task<CategoryDto> AddAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null) throw new ArgumentNullException(nameof(categoryDto));
            var category = new Category { Name = categoryDto.Name };
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            await _userNotificationConfigurationService.CreateNotificationConfigForAllUsersAsync(category.CategoryId);

            return new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            category.Name = categoryDto.Name;
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name });
        }

        public async Task<int> GetCategoryIdAsync(Article article, List<Keywords>? keywords, List<CategoryDto>? categories)
        {
            var title = article.Title?.ToLower() ?? "";
            var content = article.Content?.ToLower() ?? "";

            foreach (var keyword in keywords)
            {
                var keywordText = keyword.Keyword.ToLower();
                if (title.Contains(keywordText) || content.Contains(keywordText))
                {
                    article.CategoryId = keyword.CategoryId;
                    return keyword.CategoryId;
                }
            }

            var allCategory = categories!.FirstOrDefault(c => c.Name.Equals("All", StringComparison.OrdinalIgnoreCase));
            if (allCategory != null)
            {
                article.CategoryId = allCategory.CategoryId!.Value;
                return allCategory.CategoryId!.Value;
            }
            return 0;
        }
    }
}
