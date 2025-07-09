using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace NewsAggregation.ExternalServers.Services
{
    public static class NewsFetcherHelper
    {
        public static bool ShouldSendArticleToUser(Article article, UserReadDto? user, ICollection<UserNotificationConfigurationDto>? userConfiguration)
        {
            if (userConfiguration == null || article == null)
                return false;

            return userConfiguration.Any(config => config.CategoryId == article.CategoryId && config.IsEnabled);
        }

        public static string BuildEmailBody(UserReadDto user, List<Article> articles)
        {
            // Read the template
            var templatePath = "NewsAggregation/Templates/Email/UserArticlesDigestTemplate.html";
            var template = File.ReadAllText(templatePath);

            // Build the articles list
            var articlesList = new StringBuilder();
            foreach (var article in articles)
            {
                articlesList.AppendLine($"<li><a href='{article.Url}'>{article.Title}</a></li>");
            }

            // Replace placeholders
            var result = template
                .Replace("{{username}}", user.Username)
                .Replace("{{articles}}", articlesList.ToString());
            return result;
        }

        public static async Task SaveNotifications(int userId, List<Article>? articles, IUserNotificationService userNotificationService)
        {
            var notifications = articles.Select(article => new UserNotification
            {
                UserId = userId,
                ArticleId = article.ArticleId,
                SentDateTime = System.DateTime.UtcNow,
                IsRead = false
            }).ToList();

            await userNotificationService.AddRangeAsync(notifications);
        }
    }
} 