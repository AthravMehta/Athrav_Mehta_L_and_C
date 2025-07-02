using NewsAggregation.Constants;
using NewsAggregation.ExternalServers.Adapters;
using NewsAggregation.ExternalServers.Adapters.Contracts;
using NewsAggregation.ExternalServers.Factory.Contracts;

namespace NewsAggregation.ExternalServers.Factory
{
    public class NewsApiFactory : INewsApiFactory
    {
        public INewsApiAdapter CreateAdapter(string baseUrl)
        {
            return baseUrl switch
            {
                string s when s.Contains(AppConstants.NewsApiBase) => new NewsApiAdapter(),
                string s when s.Contains(AppConstants.TheNewsApiBase) => new TheNewsApiAdapter(),
                _ => throw new NotSupportedException(AppConstants.UnsupportedApi)
            };
        }
    }
}
