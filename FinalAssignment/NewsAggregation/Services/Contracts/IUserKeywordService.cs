using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserKeywordService
    {
        Task<UserKeywordDto> AddAsync(UserKeywordDto dto);
        Task<UserKeywordDto> UpdateAsync(int id, UserKeywordDto dto);
        Task DeleteAsync(int id);
        Task<UserKeywordDto> GetByIdAsync(int id);
        Task<IEnumerable<UserKeywordDto>> GetAllAsync();
    }
}
