using NewsAggregation.ExternalServers.Services.Contracts;

namespace NewsAggregation.ExternalServers.Services
{
    public class NewsFetchingService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<NewsFetchingService> _logger;

        public NewsFetchingService(IServiceProvider services, ILogger<NewsFetchingService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var fetcher = scope.ServiceProvider.GetRequiredService<INewsFetcher>();

                try
                {
                    await fetcher.FetchAndStoreNewsAsync();
                    _logger.LogInformation("News fetch completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching news");
                }

                await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            }
        }
    }
}