using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Services
{
    public class KeywordService
    {
        private readonly ApiService _apiService;

        public KeywordService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<KeywordDto>> GetAllKeywordsAsync()
        {
            return await _apiService.GetAsync<List<KeywordDto>>("/api/keywords/");
        }

        public async Task<MessageResponseDto> AddKeywordsAsync(List<KeywordDto> keywords)
        {
            return await _apiService.PostAsync<MessageResponseDto>("/api/keywords/", keywords);
        }

        public async Task<MessageResponseDto> HideKeywordAsync(int keywordId, string reason)
        {
            return await _apiService.PostAsync<MessageResponseDto>(
                $"/api/keywords/hide/{keywordId}", reason);
        }

        public async Task<MessageResponseDto> UnhideKeywordAsync(int keywordId)
        {
            return await _apiService.PostAsync<MessageResponseDto>(
                $"/api/keywords/unhide/{keywordId}", null);
        }
    }
}
