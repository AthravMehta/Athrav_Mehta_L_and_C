using Hangfire;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.ExternalServers.Factory.Contracts;
using NewsAggregation.ExternalServers.Services.Contracts;
using System.Text;

public class NewsFetcherService : INewsFetcher
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly INewsApiFactory _apiFactory;
    private readonly NewsAggregationDbContext _context;
    private readonly ILogger<NewsFetcherService> _logger;
    private readonly NotificationSenderFactory _notificationSenderFactory;

    public NewsFetcherService(
        IHttpClientFactory clientFactory,
        INewsApiFactory apiFactory,
        NewsAggregationDbContext context,
        ILogger<NewsFetcherService> logger,
        NotificationSenderFactory notificationSenderFactory)
    {
        _clientFactory = clientFactory;
        _apiFactory = apiFactory;
        _context = context;
        _logger = logger;
        _notificationSenderFactory = notificationSenderFactory;
    }

    public async Task FetchAndStoreNewsAsync()
    {
        var activeServers = await _context.ExternalServers
            .Where(s => s.isActive)
            .ToListAsync();

        foreach (var server in activeServers)
        {
            try
            {
                var adapter = _apiFactory.CreateAdapter(server.BaseUrl);
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("NewsAggregationApp/1.0");
                var response = await client.GetAsync(GetApiUrl(server));
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"NewsAPI error {response.StatusCode}: {errorContent}");
                    response.EnsureSuccessStatusCode();
                    continue;
                }

                var content = await response.Content.ReadAsStreamAsync();
                var articles = await adapter.ConvertToArticles(content, server.Id, GetCategoryId(server));

                await SaveArticles(articles);
                await SendNotification(articles);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                continue;
            }
        }
    }

    private string GetApiUrl(ExternalServer? server)
    {
        if(server != null)
        {
            return server.BaseUrl switch
            {
                string s when s.Contains("newsapi.org") => $"{server.BaseUrl}apiKey={server.ApiKeyHash}",
                string s when s.Contains("thenewsapi.com") => $"{server.BaseUrl}api_token={server.ApiKeyHash}",
                _ => throw new NotSupportedException("Unsupported API")
            };
        }
        return String.Empty;
    }

    private Guid GetCategoryId(ExternalServer server)
    {
        // TODO: Implement category mapping logic
        Guid.TryParse("409113C4-6F6F-4A08-5474-08DDA9DB0095", out var newGuid);
        return newGuid;
    }

    private async Task SaveArticles(IEnumerable<Article> articles)
    {
        foreach (var article in articles)
        {
            var entry = _context.Entry(article);
            _context.Articles.Add(article);
            Console.WriteLine(entry);
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save articles");
            throw;
        }
    }

    private async Task SendNotification(IEnumerable<Article> articles)
    {
        var users = await _context.Users
            .Include(u => u.NotificationConfigurations)
            .ToListAsync();

        foreach (var user in users)
        {
            var config = user.NotificationConfigurations;
            if (config == null)
                continue;

            var userArticles = articles
                .Where(a => ShouldSendArticleToUser(a, user, config))
                .ToList();

            if (!userArticles.Any())
                continue;

            var subject = "Your Personalized News Digest";
            var body = BuildEmailBody(user, userArticles);

            var sender = _notificationSenderFactory.GetSender("Email");
            BackgroundJob.Enqueue(() => sender.SendAsync(user.Email, subject, body));
        }
    }

    private bool ShouldSendArticleToUser(Article article, User user, ICollection<UserNotificationConfiguration> userConfiguration)
    {
        // TODO: Write Logic to find which articles to send.
        return  userConfiguration.FirstOrDefault(a => a.CategoryId == article.CategoryId && a.isEnabled) != null;
        //foreach (var config in userConfiguration)
        //{
        //    if (config.CategoryId != null && config.isEnabled)
        //    {
        //        return article.CategoryId == config.CategoryId;
        //    }
        //}
        //return false;
    }

    private string BuildEmailBody(User user, List<Article> articles)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>Hello {user.Username},</h2>");
        sb.AppendLine("<p>Here are your latest news articles:</p>");
        sb.AppendLine("<ul>");
        foreach (var article in articles)
        {
            sb.AppendLine($"<li><a href='{article.Url}'>{article.Title}</a></li>");
        }
        sb.AppendLine("</ul>");
        sb.AppendLine("<p>Thank you for using News Aggregation App!</p>");
        return sb.ToString();
    }

}
