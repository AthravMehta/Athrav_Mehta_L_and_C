using NewsAggregation.Entities;
using NewsAggregation.ExternalServers.Adapters.Contracts;
using System.Text.Json;

namespace NewsAggregation.ExternalServers.Adapters
{
    public class NewsApiAdapter : INewsApiAdapter
    {
        public async Task<IEnumerable<Article>> ConvertToArticles(Stream apiResponse, int externalServerId)
        {
            using var jsonDoc = await JsonDocument.ParseAsync(apiResponse);
            var root = jsonDoc.RootElement;

            var articles = new List<Article>();
            if (root.TryGetProperty("articles", out JsonElement articlesArray))
            {
                foreach (var item in articlesArray.EnumerateArray())
                {
                    Article article = ConvertToArticle(item);
                    article.ExternalServerId = externalServerId;
                    articles.Add(article);
                }
            }
            return articles;
        }
        private Article ConvertToArticle(JsonElement item)
        {
            return new Article
            {
                Title = item.GetProperty("title").GetString() ?? string.Empty,
                Content = (item.TryGetProperty("content", out JsonElement contentProp) && contentProp.ValueKind != JsonValueKind.Null
                        ? contentProp.GetString()
                        : item.GetProperty("description").GetString()) ?? string.Empty,
                Source = item.GetProperty("source").GetProperty("name").GetString() ?? string.Empty,
                Url = item.GetProperty("url").GetString() ?? string.Empty,
                PublishedDate = DateTime.TryParse(item.GetProperty("publishedAt").GetString(), out var dt) ? dt : DateTime.UtcNow
            };
        }
    }

}