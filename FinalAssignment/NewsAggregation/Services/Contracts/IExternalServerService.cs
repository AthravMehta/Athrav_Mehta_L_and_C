using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IExternalServerService
    {
        Task<ExternalServerDto> AddAsync(ExternalServerDto dto);
        Task<ExternalServerDto> UpdateAsync(int id, ExternalServerDto dto);
        Task<ExternalServerDto> GetByIdAsync(int id);
        Task<IEnumerable<ExternalServerDto>> GetAllAsync(bool? isActiveFilter = null );
    }
}
