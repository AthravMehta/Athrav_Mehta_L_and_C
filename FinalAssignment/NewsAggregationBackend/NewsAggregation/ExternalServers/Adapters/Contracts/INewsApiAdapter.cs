using NewsAggregation.Entities;

namespace NewsAggregation.ExternalServers.Adapters.Contracts
{
    public interface INewsApiAdapter
    {
        public Task<IEnumerable<Article>> ConvertToArticles(Stream apiResponse, int externalServerId);
    }
}
