using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

namespace NewsAggregationConsole.Flows
{
    public static class SavedArticlesFlow
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            var articleService = new ArticleService(apiService);
            var categoryService = new CategoryService(apiService);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Saved Articles ===\n");

                var savedArticles = await articleService.GetSavedArticleForUserAsync();

                if (savedArticles == null || !savedArticles.Any())
                {
                    Console.WriteLine("No saved articles found.");
                }
                else
                {
                    DisplayHelper.DisplayArticles(savedArticles);
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Back");
                Console.WriteLine("2. Delete Saved Article");
                Console.WriteLine("3. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        return;
                    case 2:
                        if (savedArticles == null || !savedArticles.Any())
                        {
                            Console.WriteLine("No articles to delete. Press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }

                        int articleIdToDelete = InputHelper.GetInt("Enter Article ID to delete: ");
                        var articleToDelete = savedArticles.FirstOrDefault(a => a.ArticleId == articleIdToDelete);
                        if (articleToDelete == null)
                        {
                            Console.WriteLine("Article not found in your saved articles.");
                        }
                        else
                        {
                            ToggleSaveResponseDto deleted = await articleService.SaveArticleForUserAsync(articleIdToDelete);
                            Console.WriteLine(deleted.Message);
                        }
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
