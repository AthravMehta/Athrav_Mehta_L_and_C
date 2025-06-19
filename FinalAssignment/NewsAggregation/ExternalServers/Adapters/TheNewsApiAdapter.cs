using NewsAggregation.Entities;
using NewsAggregation.ExternalServers.Adapters.Contracts;
using System.Text.Json;

namespace NewsAggregation.ExternalServers.Adapters
{
    public class TheNewsApiAdapter : INewsApiAdapter
    {
        public async Task<IEnumerable<Article>> ConvertToArticles(Stream apiResponseStream, int externalServerId, int categoryId)
        {
            var articles = new List<Article>();

            using var jsonDoc = await JsonDocument.ParseAsync(apiResponseStream);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("data", out JsonElement dataArray))
            {
                foreach (var item in dataArray.EnumerateArray())
                {
                    articles.Add(new Article
                    {
                        Title = item.GetProperty("title").GetString() ?? string.Empty,
                        Content = item.GetProperty("description").GetString() ?? string.Empty,
                        Source = item.GetProperty("source").GetString() ?? string.Empty,
                        Url = item.GetProperty("url").GetString() ?? string.Empty,
                        ExternalServerId = externalServerId,
                        CategoryId = categoryId,
                        PublishedDate = DateTime.TryParse(item.GetProperty("published_at").GetString(), out var dt) ? dt : DateTime.UtcNow
                    });
                }
            }

            return articles;
        }
    }
}