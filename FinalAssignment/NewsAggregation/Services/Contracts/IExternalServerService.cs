using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IExternalServerService
    {
        Task<ExternalServerDto> AddAsync(ExternalServerDto dto);
        Task<ExternalServerDto> UpdateAsync(Guid id, ExternalServerDto dto);
        Task DeleteAsync(Guid id);
        Task<ExternalServerDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ExternalServerDto>> GetAllAsync();
    }
}
