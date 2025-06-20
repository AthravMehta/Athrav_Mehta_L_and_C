using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

namespace NewsAggregationConsole.Flows
{
    public static class SearchFlow
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            var articleService = new ArticleService(apiService);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Search Articles ===\n");

                string searchText = InputHelper.GetString("Enter Search text: ");

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    Console.WriteLine("Search text cannot be empty. Press Enter to try again...");
                    Console.ReadLine();
                    continue;
                }

                string dateRangeChoice = InputHelper.GetString("Do you want to filter by date range? (y/n): ").ToLower();

                DateTime? startDate = null;
                DateTime? endDate = null;

                if (dateRangeChoice == "y" || dateRangeChoice == "yes")
                {
                    startDate = InputHelper.GetDate("Enter start date (yyyy-MM-dd): ");
                    endDate = InputHelper.GetDate("Enter end date (yyyy-MM-dd): ");

                    if (endDate < startDate)
                    {
                        Console.WriteLine("End date cannot be earlier than start date. Press Enter to try again...");
                        Console.ReadLine();
                        continue;
                    }
                }

                Console.WriteLine("\nSort by:");
                Console.WriteLine("1. Likes");
                Console.WriteLine("2. Dislikes");
                Console.WriteLine("3. No Sorting");

                int sortChoice = InputHelper.GetInt("Choose sorting option: ", 1, 3);

                var queryOptions = new ArticleQueryDto
                {
                    SearchText = searchText,
                    StartDate = startDate,
                    EndDate = endDate,
                    SortByLikes = sortChoice == 1,
                    SortByDislikes = sortChoice == 2
                };

                var articles = await articleService.GetFilteredArticlesAsync(queryOptions);

                if (articles == null || !articles.Any())
                {
                    Console.WriteLine("\nNo articles found matching your criteria.");
                }
                else
                {
                    Console.WriteLine($"\nFound {articles.Count} article(s):\n");

                    DisplayHelper.DisplayArticles(articles);
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Back");
                Console.WriteLine("2. Save Article");
                Console.WriteLine("3. Logout");

                int option = InputHelper.GetInt("Choose option: ", 1, 3);

                switch (option)
                {
                    case 1:
                        return;

                    case 2:
                        if (articles == null || !articles.Any())
                        {
                            Console.WriteLine("No articles to save. Press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }

                        int articleIdToSave = InputHelper.GetInt("Enter Article ID to save: ");
                        var articleToSave = articles.FirstOrDefault(a => a.ArticleId == articleIdToSave);
                        if (articleToSave == null)
                        {
                            Console.WriteLine("Article ID not found in the search results.");
                        }
                        else
                        {
                            var savedResponse = await articleService.SaveArticleForUserAsync(articleIdToSave);
                            Console.WriteLine(savedResponse.Message);
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
