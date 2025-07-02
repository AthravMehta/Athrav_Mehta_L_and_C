using Hangfire;
using NewsAggregation.Entities;
using NewsAggregation.ExternalServers.Factory.Contracts;
using NewsAggregation.ExternalServers.Services.Contracts;
using NewsAggregation.Models;
using NewsAggregation.Notifications;
using NewsAggregation.Services;
using NewsAggregation.Services.Contracts;
using System.Text;

namespace NewsAggregation.ExternalServers.Services
{
    public class NewsFetcherService : INewsFetcher
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly INewsApiFactory _apiFactory;
        private readonly ILogger<NewsFetcherService> _logger;
        private readonly NotificationSenderFactory _notificationSenderFactory;
        private readonly EncryptionService _encryptionService;
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleSerivce;
        private readonly IUserService _userService;
        private readonly IExternalServerService _externalServerService;
        private readonly IKeywordService _keywordService;
        private readonly IUserNotificationService _userNotificationService;

        public NewsFetcherService(
            IHttpClientFactory clientFactory,
            INewsApiFactory apiFactory,
            ILogger<NewsFetcherService> logger,
            NotificationSenderFactory notificationSenderFactory,
            EncryptionService encryptionService,
            ICategoryService categoryService,
            IArticleService articleService,
            IUserService userService,
            IExternalServerService externalServerService,
            IKeywordService keywordService,
            IUserNotificationService userNotificationService)
        {
            _clientFactory = clientFactory;
            _apiFactory = apiFactory;
            _logger = logger;
            _notificationSenderFactory = notificationSenderFactory;
            _encryptionService = encryptionService;
            _categoryService = categoryService;
            _articleSerivce = articleService;
            _userService = userService;
            _externalServerService = externalServerService;
            _keywordService = keywordService;
            _userNotificationService = userNotificationService;
        }

        public async Task FetchAndStoreNewsAsync()
        {
            var activeServers = await _externalServerService.GetAllAsync(isActiveFilter: true);

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
                    var articles = await adapter.ConvertToArticles(content, server.ExternalServerId!.Value);
                    articles = await GetArticlesCategory(articles);

                    articles = await SaveArticles(articles);
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

        private string GetApiUrl(ExternalServerDto? server)
        {
            if (server != null)
            {
               var apiKey = GetDecryptedApiKey(server.ApiKeyHash);
                return server.BaseUrl switch
                {
                    string s when s.Contains("newsapi.org") => $"{server.BaseUrl}{apiKey}",
                    string s when s.Contains("thenewsapi.com") => $"{server.BaseUrl}{apiKey}",
                    _ => throw new NotSupportedException("Unsupported API")
                };
            }
            return String.Empty;
        }

        private string GetDecryptedApiKey(string hashedApiKey)
        {
            return _encryptionService.Decrypt(hashedApiKey);
        }

        private async Task<List<Article>> GetArticlesCategory(IEnumerable<Article> articles)
        {
            List<Keywords>? keywords = await _keywordService.GetAllKeywordsAsync();
            List<CategoryDto>? categories = (await _categoryService.GetAllAsync()).ToList();

            var updatedArticles = new List<Article>();

            foreach (var article in articles)
            {
                article.CategoryId = await _categoryService.GetCategoryIdAsync(article, keywords, categories);
                updatedArticles.Add(article);
            }

            return updatedArticles;
        }


        private async Task<IEnumerable<Article>> SaveArticles(IEnumerable<Article> articles)
        {
            try
            {
                return await _articleSerivce.AddAllArticlesAsync(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save articles");
                throw;
            }
        }

        private async Task SendNotification(IEnumerable<Article> articles)
        {
            var users = await _userService.GetAllUsersAsync();

            foreach (var user in users)
            {
                var config = user.UserNotificationConfigurations;
                if (config == null || !config.Any())
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

                await SaveNotifications(user.UserId, userArticles);
            }
        }

        private async Task SaveNotifications(int userId, List<Article>? articles)
        {
            var notifications = articles.Select(article => new UserNotification
            {
                UserId = userId,
                ArticleId = article.ArticleId,
                SentDateTime = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            await _userNotificationService.AddRangeAsync(notifications);
        }

        private bool ShouldSendArticleToUser(Article article, UserReadDto? user, ICollection<UserNotificationConfigurationDto>? userConfiguration)
        {
            if (userConfiguration == null || article == null)
                return false;

            return userConfiguration.Any(config => config.CategoryId == article.CategoryId && config.IsEnabled);
        }

        private string BuildEmailBody(UserReadDto user, List<Article> articles)
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
}