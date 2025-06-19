using NewsAggregation.Entities;
using System.Threading.Tasks;

namespace NewsAggregation.Services.Contracts
{
    public interface IKeywordService
    {
        Task<List<Keywords>> GetAllKeywordsAsync();
    }
}
