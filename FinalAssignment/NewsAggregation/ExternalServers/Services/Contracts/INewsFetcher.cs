using NewsAggregation.Entities;

namespace NewsAggregation.ExternalServers.Services.Contracts
{
    public interface INewsFetcher
    {
        Task FetchAndStoreNewsAsync();
    }
}
