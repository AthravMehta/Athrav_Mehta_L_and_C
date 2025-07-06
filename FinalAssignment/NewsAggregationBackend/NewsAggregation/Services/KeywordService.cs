using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly IArticleService _articleService;
        private readonly ICrudBaseRepository<Keywords> _keywordRepo;

        public KeywordService(IArticleService articleService, ICrudBaseRepository<Keywords> keywordRepo)
        {
            _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
            _keywordRepo = keywordRepo ?? throw new ArgumentNullException(nameof(keywordRepo));
        }

        public async Task<List<Keywords>> GetAllKeywordsAsync()
        {
            var keywordsEnumerable = await _keywordRepo.GetAllAsync();
            if (keywordsEnumerable == null || !keywordsEnumerable.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return keywordsEnumerable.ToList();
        }

        public async Task<bool> HideKeywordAsync(int keywordId, string reason)
        {
            var keyword = await _keywordRepo.GetByIdAsync(keywordId);
            if (keyword == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            keyword.IsHidden = true;
            keyword.HideReason = reason;
            await _keywordRepo.UpdateAsync(keyword);
            await _keywordRepo.SaveChangesAsync();

            await _articleService.HideArticlesByKeywordAsync(keywordId);
            return true;
        }

        public async Task<bool> UnhideKeywordAsync(int keywordId)
        {
            var keyword = await _keywordRepo.GetByIdAsync(keywordId);
            if (keyword == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            keyword.IsHidden = false;
            keyword.HideReason = null;
            await _keywordRepo.UpdateAsync(keyword);
            await _keywordRepo.SaveChangesAsync();

            await _articleService.UnhideArticlesByKeywordAsync(keywordId);
            return true;
        }
    }
}
