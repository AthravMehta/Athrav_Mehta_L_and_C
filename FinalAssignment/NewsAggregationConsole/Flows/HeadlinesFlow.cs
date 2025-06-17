using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

namespace NewsAggregationConsole.Flows
{
    public static class HeadlinesFlow
    {
        public static async Task Run(UserDto currentUser, ApiService apiService)
        {
            var articleService = new ArticleService(apiService);

            while (true)
            {
                Console.Clear();
                DisplayHelper.ShowHeader(currentUser);
                Console.WriteLine("Headlines Menu");
                Console.WriteLine("--------------");
                Console.WriteLine("1. Today");
                Console.WriteLine("2. Date Range");
                Console.WriteLine("3. Back");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        await ShowArticlesForToday(articleService, currentUser);
                        break;
                    case 2:
                        await ShowArticlesForDateRange(articleService, currentUser);
                        break;
                    case 3:
                        return;
                }
            }
        }

        private static async Task ShowArticlesForToday(ArticleService articleService, UserDto user)
        {
            var today = DateTime.Today;
            var articles = await articleService.GetArticlesByDateRangeAsync(today, today);
            await ShowArticlesPage(articles, user, articleService);
        }

        private static async Task ShowArticlesForDateRange(ArticleService articleService, UserDto user)
        {
            Console.Clear();
            Console.WriteLine("Enter Start Date (yyyy-MM-dd):");
            DateTime startDate = InputHelper.GetDate("Start Date: ");
            Console.WriteLine("Enter End Date (yyyy-MM-dd):");
            DateTime endDate = InputHelper.GetDate("End Date: ");

            Console.WriteLine("\nChoose Category:");
            Console.WriteLine("1. All");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. Entertainment");
            Console.WriteLine("4. Sports");
            Console.WriteLine("5. Technology");

            int catChoice = InputHelper.GetInt("Select category: ", 1, 5);
            string category = catChoice switch
            {
                1 => "all",
                2 => "business",
                3 => "entertainment",
                4 => "sports",
                5 => "technology",
                _ => "all"
            };

            var articles = await articleService.GetArticlesByDateRangeAsync(startDate, endDate);
            await ShowArticlesPage(articles, user, articleService);
        }

        private static async Task ShowArticlesPage(List<ArticleDto> articles, UserDto user, ArticleService articleService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Articles Found: {articles.Count}\n");
                Console.WriteLine("ID | Headline");
                Console.WriteLine("------------------------------");
                foreach (var article in articles)
                {
                    Console.WriteLine($"{article.Id} | {article.Title}");
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. View Article Details by ID");
                Console.WriteLine("2. Back");
                Console.WriteLine("3. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                if (choice == 1)
                {
                    int articleId = InputHelper.GetInt("Enter Article ID: ");
                    var selectedArticle = articles.FirstOrDefault(a => a.Id == articleId);
                    if (selectedArticle == null)
                    {
                        Console.WriteLine("Invalid Article ID. Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }
                    await ShowArticleDetails(selectedArticle, user, articleService);
                }
                else if (choice == 2)
                {
                    return; // Back to previous menu
                }
                else if (choice == 3)
                {
                    Environment.Exit(0); // Logout and exit app or redirect as needed
                }
            }
        }

        private static async Task ShowArticleDetails(ArticleDto article, UserDto user, ArticleService articleService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Article ID: {article.Id}");
                Console.WriteLine($"Headline: {article.Title}");
                Console.WriteLine($"Source: {article.Source}");
                Console.WriteLine($"Category: {article.CategoryId}");
                Console.WriteLine($"Published Date: {article.PublishedDate:yyyy-MM-dd}");
                Console.WriteLine($"URL: {article.Url}\n");
                Console.WriteLine("Content:");
                Console.WriteLine(article.Content);
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Save Article");
                Console.WriteLine("2. Back");
                Console.WriteLine("3. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                if (choice == 1)
                {
                    bool saved = await articleService.SaveArticleForUserAsync(article.Id);
                    Console.WriteLine(saved ? "Article saved successfully!" : "Article unsaved successfully.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                else if (choice == 2)
                {
                    return;
                }
                else if (choice == 3)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
