using Hangfire;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.ExternalServers.Factory.Contracts;
using NewsAggregation.ExternalServers.Services.Contracts;
using NewsAggregation.Models;
using NewsAggregation.Notifications;
using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Services.Contracts;
using System.Text;

namespace NewsAggregation.ExternalServers.Services
{
    public class NewsFetcherService : INewsFetcher
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly INewsApiFactory _apiFactory;
        private readonly ILogger<NewsFetcherService> _logger;
        private readonly INotificationSenderFactory _notificationSenderFactory;
        private readonly IEncryptionService _encryptionService;
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
            INotificationSenderFactory notificationSenderFactory,
            IEncryptionService encryptionService,
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
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(AppConstants.NewsAggregationAppVersion);
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
                    string s when s.Contains(AppConstants.NewsApiBase) => $"{server.BaseUrl}{apiKey}",
                    string s when s.Contains(AppConstants.TheNewsApiBase) => $"{server.BaseUrl}{apiKey}",
                    _ => throw new NotSupportedException(ErrorMessages.UnsupportedApi)
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
                    .Where(a => NewsFetcherHelper.ShouldSendArticleToUser(a, user, config))
                    .ToList();

                if (!userArticles.Any())
                    continue;

                var subject = AppConstants.UserArticleEmailSubject;
                var body = NewsFetcherHelper.BuildEmailBody(user, userArticles);

                var sender = _notificationSenderFactory.GetSender(NotificationType.Email);
                BackgroundJob.Enqueue(() => sender.SendAsync(user.Email, subject, body));

                await NewsFetcherHelper.SaveNotifications(user.UserId, userArticles, _userNotificationService);
            }
        }

    }
}