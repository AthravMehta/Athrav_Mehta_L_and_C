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
                string s when s.Contains("newsapi.org") => new NewsApiAdapter(),
                string s when s.Contains("thenewsapi.com") => new TheNewsApiAdapter(),
                _ => throw new NotSupportedException("Unsupported API")
            };
        }
    }
}
