using NewsAggregation.ExternalServers.Adapters.Contracts;

namespace NewsAggregation.ExternalServers.Factory.Contracts
{
    public interface INewsApiFactory
    {
        public INewsApiAdapter CreateAdapter(string baseUrl);
    }
}
