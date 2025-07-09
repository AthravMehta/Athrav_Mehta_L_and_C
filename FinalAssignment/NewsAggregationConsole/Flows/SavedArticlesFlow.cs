using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

public static class SavedArticlesFlow
{
    public static async Task Run(UserReadDto currentUser, ApiService apiService)
    {
        var articleService = new ArticleService(apiService);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Saved Articles ===\n");

            var savedArticles = await articleService.GetSavedArticleForUserAsync();

            if (savedArticles == null || !savedArticles.Any())
            {
                Console.WriteLine("No saved articles found.");
                Console.WriteLine("Press Enter to go back...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Articles Found: {savedArticles.Count}\n");
            Console.WriteLine("ID | Headline");
            Console.WriteLine("------------------------------");
            foreach (var article in savedArticles)
            {
                Console.WriteLine($"{article.ArticleId} | {article.Title}");
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. View Article Details by ID");
            Console.WriteLine("2. Back");
            Console.WriteLine("3. Logout");

            int choice = InputHelper.GetInt("Choose option: ", 1, 3);

            switch (choice)
            {
                case 1:
                    int articleId = InputHelper.GetInt("Enter Article ID: ");
                    var selectedArticle = await articleService.GetArticleByIdAsync(articleId);

                    if (selectedArticle == null)
                    {
                        Console.WriteLine("Invalid Article ID. Press Enter to continue...");
                        Console.ReadLine();
                        break;
                    }

                    await ArticleFlowHelper.ShowArticleDetails(selectedArticle, currentUser, articleService, allowSave: false);
                    break;

                case 2:
                    return;

                case 3:
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid choice. Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }

}