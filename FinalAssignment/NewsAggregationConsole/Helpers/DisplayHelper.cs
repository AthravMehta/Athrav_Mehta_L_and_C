using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

namespace NewsAggregationConsole.Helpers
{
    public static class DisplayHelper
    {
        public static void ShowHeader(UserReadDto user, bool isAdmin = false)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Welcome, {user.Username}{(isAdmin ? " (Admin)" : "")}!");
            Console.WriteLine($"Date: {DateTime.Now:dd-MMM-yyyy}  Time: {DateTime.Now:hh:mm tt}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 40));
        }

        public static void DisplayAllCategoryAdmin(List<CategoryDto> categoryDtos)
        {
            Console.Clear();
            Console.WriteLine("All News Categories: ");
            Console.WriteLine();
            foreach (CategoryDto categoryDto in categoryDtos)
            {
                Console.WriteLine(categoryDto.Name);
            }
            Console.ReadKey();
        }

        public static void DisplayAllCategoriesWithKeywords(List<CategoryWithKeywordsDto> categories)
        {
            Console.Clear();
            Console.WriteLine("All News Categories and Keywords:");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} | {1,-20} | {2,-7} | {3,-20} || {4,-5} | {5,-20} | {6,-7} | {7,-20}",
                "CatID", "Category", "Hidden", "Hide Reason", "KeyID", "Keyword", "Hidden", "Hide Reason");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            foreach (var cat in categories)
            {
                if (cat.Keywords != null && cat.Keywords.Count > 0)
                {
                    var firstKey = cat.Keywords[0];
                    Console.WriteLine("{0,-5} | {1,-20} | {2,-7} | {3,-20} || {4,-5} | {5,-20} | {6,-7} | {7,-20}",
                        cat.CategoryId, cat.Name, cat.IsHidden ? "Yes" : "No", cat.HideReason ?? "N/A",
                        firstKey.KeywordId, firstKey.Keyword, firstKey.IsHidden!.Value ? "Yes" : "No", firstKey.HideReason ?? "N/A");
                    for (int i = 1; i < cat.Keywords.Count; i++)
                    {
                        var k = cat.Keywords[i];
                        Console.WriteLine("{0,-5} | {1,-20} | {2,-7} | {3,-20} || {4,-5} | {5,-20} | {6,-7} | {7,-20}",
                            "", "", "", "",
                            k.KeywordId, k.Keyword, k.IsHidden!.Value ? "Yes" : "No", k.HideReason ?? "N/A");
                    }
                }
                else
                {
                    Console.WriteLine("{0,-5} | {1,-20} | {2,-7} | {3,-20} || {4,-5} | {5,-20} | {6,-7} | {7,-20}",
                        cat.CategoryId, cat.Name, cat.IsHidden ? "Yes" : "No", cat.HideReason ?? "N/A",
                        "", "", "", "");
                }
                Console.WriteLine("-----------------------------------------------------------------------------------------------");
            }
        }

        public static async Task HandleCategoryHideUnhideAsync(CategoryService categoryService, List<CategoryWithKeywordsDto> categories)
        {
            int categoryId = InputHelper.GetInt("Enter Category ID to toggle hide/unhide: ");
            var category = categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                InputHelper.ShowError("Category not found!");
                return;
            }
            if (category.IsHidden)
            {
                await categoryService.UnhideCategoryAsync(categoryId);
                InputHelper.ShowSuccess("Category unhidden successfully!");
            }
            else
            {
                string reason = InputHelper.GetString("Enter hide reason: ");
                await categoryService.HideCategoryAsync(categoryId, reason);
                InputHelper.ShowSuccess("Category hidden successfully!");
            }
        }

        public static async Task HandleKeywordHideUnhideAsync(KeywordService keywordService, List<CategoryWithKeywordsDto> categories)
        {
            int keywordId = InputHelper.GetInt("Enter Keyword ID to toggle hide/unhide: ");
            var keyword = categories.SelectMany(c => c.Keywords).FirstOrDefault(k => k.KeywordId == keywordId);
            if (keyword == null)
            {
                InputHelper.ShowError("Keyword not found!");
                return;
            }
            if (keyword.IsHidden!.Value)
            {
                await keywordService.UnhideKeywordAsync(keywordId);
                InputHelper.ShowSuccess("Keyword unhidden successfully!");
            }
            else
            {
                string reason = InputHelper.GetString("Enter hide reason: ");
                await keywordService.HideKeywordAsync(keywordId, reason);
                InputHelper.ShowSuccess("Keyword hidden successfully!");
            }
        }


        public static void ShowExternalServers(List<ExternalServerDto> servers)
        {
            Console.Clear();
            Console.WriteLine("External Servers List:");
            Console.WriteLine("---------------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-10} | {1,-20} | {2,-6} | {3,-20}", "ID", "Name", "Active", "Last Accessed");
            Console.WriteLine("---------------------------------------------------------------------------------------------");

            foreach (var s in servers)
            {
                Console.WriteLine(
                    "{0,-10} | {1,-20} | {2,-6} | {3,-20}",
                    s.ExternalServerId,
                    s.ServerName,
                    s.IsActive ? "Yes" : "No",
                    s.ModifiedDateTime?.ToString("yyyy-MM-dd HH:mm:ss")
                );
            }
            Console.WriteLine("---------------------------------------------------------------------------------------------");
        }


        public static void ShowExternalServerDetails(ExternalServerDto server)
        {
            Console.Clear();
            Console.WriteLine("External Server Details:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"ID           : {server.ExternalServerId}");
            Console.WriteLine($"Name         : {server.ServerName}");
            Console.WriteLine($"Base URL     : {server.BaseUrl}");
            Console.WriteLine($"Active       : {(server.IsActive ? "Yes" : "No")}");
            Console.WriteLine($"API Key Hash : {server.ApiKeyHash}");
            Console.WriteLine($"Last Accessed: {server.ModifiedDateTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine("------------------------------------------------");
        }
        public static void ShowUpdateExternalServerPrompt(ExternalServerDto server)
        {
            Console.Clear();
            Console.WriteLine("Update External Server:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"ID           : {server.ExternalServerId}");
            Console.WriteLine($"Name         : {server.ServerName}");
            Console.WriteLine($"Base URL     : {server.BaseUrl}");
            Console.WriteLine($"Active       : {(server.IsActive ? "Yes" : "No")}");
            Console.WriteLine($"API Key Hash : {server.ApiKeyHash}");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Leave input blank to keep the current value.");
        }

        public static void ShowNotifications(List<NotificationDto> notifications)
        {
            Console.Clear();
            Console.WriteLine("Unviewed Notifications:");
            if (!notifications.Any())
            {
                Console.WriteLine("No new notifications.");
                return;
            }
            foreach (var n in notifications)
            {
                Console.WriteLine($"[{n.UserNotificationId}] --> ");
                DisplayHelper.DisplayArticleDetails(n.articleDto);
            }
            Console.WriteLine();
        }

        public static void DisplayArticles(IEnumerable<ArticleDto> articles)
        {
            foreach (var article in articles)
            {
                Console.WriteLine($"Article ID: {article.ArticleId}");
                Console.WriteLine($"Headline: {article.Title}");
                Console.WriteLine($"Source: {article.Source}");
                Console.WriteLine($"Category: {article.CategoryId}");
                Console.WriteLine($"Published Date: {article.PublishedDate:yyyy-MM-dd}");
                Console.WriteLine($"URL: {article.Url}");
                Console.WriteLine("Content:");
                Console.WriteLine(article.Content);
                Console.WriteLine(new string('-', 50));
            }
        }

        public static void DisplayArticleDetails(ArticleDto article)
        {
            Console.WriteLine($"Article ID: {article.ArticleId}");
            Console.WriteLine($"Headline: {article.Title}");
            Console.WriteLine($"Source: {article.Source}");
            Console.WriteLine($"Category: {article.CategoryId}");
            Console.WriteLine($"Published Date: {article.PublishedDate:yyyy-MM-dd}");
            Console.WriteLine($"URL: {article.Url}");
            Console.WriteLine("Content:");
            Console.WriteLine(article.Content);
            Console.WriteLine(new string('-', 50));
        }


        public static void ShowNotificationConfig(List<NotificationConfigurationDto> configs)
        {
            Console.Clear();
            Console.WriteLine("Notification Configuration:");
            int i = 1;
            foreach (var config in configs)
            {
                Console.WriteLine($"{i++}. {config.Category!.Name} - {(config.IsEnabled ? "Enabled" : "Disabled")}");
            }
            Console.WriteLine();
        }
    }
}
