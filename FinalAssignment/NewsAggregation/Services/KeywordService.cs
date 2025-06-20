using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly ICrudBaseRepository<Keywords> _keywordRepo;

        public KeywordService(ICrudBaseRepository<Keywords> keywordRepo)
        {
            _keywordRepo = keywordRepo;
        }

        public async Task<List<Keywords>> GetAllKeywordsAsync()
        {
            IEnumerable<Keywords> keywordsEnumerable = await _keywordRepo.GetAllAsync();
            var keywordsList = keywordsEnumerable.ToList();
            return keywordsList;
        }
    }
}
