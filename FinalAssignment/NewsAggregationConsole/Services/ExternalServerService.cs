using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Services
{
    public class ExternalServerService
    {
        private readonly ApiService _apiService;

        public ExternalServerService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task AddExternalAsync(ExternalServerDto externalServerDto)
        {
            await _apiService.PostAsync<ExternalServerDto>("/api/externalserver", externalServerDto);
        }

        public async Task<List<ExternalServerDto>> GetAllExternalServerAsync()
        {
            return await _apiService.GetAsync<List<ExternalServerDto>>("/api/externalserver");
        }

        public async Task<ExternalServerDto> GetExternalServerByIdAsync(Guid id)
        {
            return await _apiService.GetAsync<ExternalServerDto>($"/api/externalserver/{id}");
        }
            
        public async Task UpdateExternalServerAsync(Guid id, ExternalServerDto dto)
        {
            await _apiService.PutAsync<ExternalServerDto>($"/api/externalserver/{id}", dto);
        }
    }
}
