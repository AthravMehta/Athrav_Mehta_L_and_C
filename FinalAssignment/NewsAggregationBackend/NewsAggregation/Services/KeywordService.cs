using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly IArticleService _articleService;
        private readonly ICrudBaseRepository<Keywords> _keywordRepo;

        public KeywordService(IArticleService articleService, ICrudBaseRepository<Keywords> keywordRepo)
        {
            _articleService = articleService;
            _keywordRepo = keywordRepo;
        }

        public async Task<List<Keywords>> GetAllKeywordsAsync()
        {
            IEnumerable<Keywords> keywordsEnumerable = await _keywordRepo.GetAllAsync();
            var keywordsList = keywordsEnumerable.ToList();
            return keywordsList;
        }

        public async Task<bool> HideKeywordAsync(int keywordId, string reason)
        {
            var keyword = await _keywordRepo.GetByIdAsync(keywordId);
            if (keyword == null) return false;
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
            if (keyword == null) return false;
            keyword.IsHidden = false;
            keyword.HideReason = null;
            await _keywordRepo.UpdateAsync(keyword);
            await _keywordRepo.SaveChangesAsync();

            await _articleService.UnhideArticlesByKeywordAsync(keywordId);
            return true;
        }
    }
}
