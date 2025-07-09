using AutoMapper;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IArticleService _articleService;
        private readonly IKeywordService _keywordService;
        private readonly ICrudBaseRepository<User> _userRepository;
        private readonly ICrudBaseRepository<Category> _categoryRepository;
        private readonly IUserNotificationConfigurationService _userNotificationConfigurationService;

        public CategoryService(
            IMapper mapper,
            IArticleService articleService,
            ICrudBaseRepository<Category> categoryRepository,
            IKeywordService keywordService,
            IUserNotificationConfigurationService userNotificationConfigurationService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _keywordService = keywordService ?? throw new ArgumentNullException(nameof(keywordService));
            _userNotificationConfigurationService = userNotificationConfigurationService ?? throw new ArgumentNullException(nameof(userNotificationConfigurationService));
        }

        public async Task<CategoryDto> AddAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var category = new Category { Name = categoryDto.Name };
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            await _userNotificationConfigurationService.CreateNotificationConfigForAllUsersAsync(category.CategoryId);

            return new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            category.Name = categoryDto.Name;
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories == null || !categories.Any())
                return Enumerable.Empty<CategoryDto>();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<int> GetCategoryIdAsync(Article article, List<Keywords>? keywords, List<CategoryDto>? categories)
        {
            if (article == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var title = article.Title?.ToLower() ?? "";
            var content = article.Content?.ToLower() ?? "";

            if (keywords != null)
            {
                foreach (var keyword in keywords)
                {
                    var keywordText = keyword.Keyword.ToLower();
                    if (title.Contains(keywordText) || content.Contains(keywordText))
                    {
                        article.CategoryId = keyword.CategoryId;
                        return keyword.CategoryId;
                    }
                }
            }

            if (categories != null)
            {
                var allCategory = categories.FirstOrDefault(c => c.Name.Equals("All", StringComparison.OrdinalIgnoreCase));
                if (allCategory != null && allCategory.CategoryId.HasValue)
                {
                    article.CategoryId = allCategory.CategoryId.Value;
                    return allCategory.CategoryId.Value;
                }
            }
            return 0;
        }

        public async Task<bool> HideCategoryAsync(int categoryId, string reason)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            category.IsHidden = true;
            category.HideReason = reason;
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var articleQueryDto = new ArticleQueryDto
            {
                CategoryId = categoryId,
                IsHidden = false
            };
            await _articleService.HideArticlesByCategoryAsync(articleQueryDto);
            return true;
        }

        public async Task<bool> UnhideCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            category.IsHidden = false;
            category.HideReason = null;
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var articleQueryDto = new ArticleQueryDto
            {
                CategoryId = categoryId,
                IsHidden = true,
                HideReason = HideReasonEnum.AdminHiddenCategory
            };

            await _articleService.UnhideArticlesByCategoryAsync(articleQueryDto);
            return true;
        }
    }
}
