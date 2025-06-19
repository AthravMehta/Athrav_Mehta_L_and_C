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
            try
            {
                IEnumerable<Keywords> keywordsEnumerable = await _keywordRepo.GetAllAsync();
                var keywordsList = keywordsEnumerable.ToList();
                return keywordsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching keywords: {ex.Message}");
                return new List<Keywords>();
            }
        }

    }
}
