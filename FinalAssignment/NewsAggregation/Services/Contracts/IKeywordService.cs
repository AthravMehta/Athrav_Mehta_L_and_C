using NewsAggregation.Entities;
using System.Threading.Tasks;

namespace NewsAggregation.Services.Contracts
{
    public interface IKeywordService
    {
        Task<List<Keywords>> GetAllKeywordsAsync();
        Task<bool> HideKeywordAsync(int keywordId, string reason);
        Task<bool> UnhideKeywordAsync(int keywordId);
    }
}
