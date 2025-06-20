using NewsAggregationConsole.Enums;
using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

namespace NewsAggregationConsole.Flows
{
    public static class HeadlinesFlow
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            var articleService = new ArticleService(apiService);
            var categoryService = new CategoryService(apiService);

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
                        await ShowArticlesForDateRange(articleService, categoryService, currentUser);
                        break;
                    case 3:
                        return;
                }
            }
        }

        private static async Task ShowArticlesForToday(ArticleService articleService, UserReadDto user)
        {
            var today = DateTime.Today;
            var query = new ArticleQueryDto
            {
                StartDate = today,
                EndDate = today,
                CategoryId = null
            };
            var articles = await articleService.GetFilteredArticlesAsync(query);
            await ShowArticlesPage(articles, user, articleService);
        }

        private static async Task ShowArticlesForDateRange(ArticleService articleService, CategoryService categoryService, UserReadDto user)
        {
            Console.Clear();
            Console.WriteLine("Enter Start Date (yyyy-MM-dd):");
            DateTime startDate = InputHelper.GetDate("Start Date: ");
            Console.WriteLine("Enter End Date (yyyy-MM-dd):");
            DateTime endDate = InputHelper.GetDate("End Date: ");

            Console.WriteLine("\nChoose Category:");
            var categories = await categoryService.GetAllCategoryAsync();

            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i].Name}");
            }

            int categoryChoice = InputHelper.GetInt("Select category: ", 1, categories.Count);
            CategoryDto selectedCategory = categories[categoryChoice - 1];

            var query = new ArticleQueryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                CategoryId = selectedCategory.CategoryId
            };

            List<ArticleDto> articles;
            if (selectedCategory.CategoryId == 0)
            {
                articles = await articleService.GetFilteredArticlesAsync(query);
            }
            else
            {
                articles = await articleService.GetFilteredArticlesAsync(query);
            }

            await ShowArticlesPage(articles, user, articleService);
        }

        private static async Task ShowArticlesPage(List<ArticleDto> articles, UserReadDto user, ArticleService articleService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Articles Found: {articles.Count}\n");
                Console.WriteLine("ID | Headline");
                Console.WriteLine("------------------------------");

                if (!articles.Any())
                {
                    Console.WriteLine("No Articles Found!!");
                    continue;
                }
                foreach (var article in articles)
                {
                    Console.WriteLine($"{article.ArticleId} | {article.Title}");
                }

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. View Article Details by ID");
                Console.WriteLine("2. Back");
                Console.WriteLine("3. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                if (choice == 1)
                {
                    int articleId = InputHelper.GetInt("Enter Article ID: ");
                    var selectedArticle = articles.FirstOrDefault(a => a.ArticleId == articleId);
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
                    return;
                }
                else if (choice == 3)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static async Task ShowArticleDetails(ArticleDto article, UserReadDto user, ArticleService articleService)
        {
            while (true)
            {
                Console.Clear();
                DisplayHelper.DisplayArticleDetails(article);
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Save Article");
                Console.WriteLine("2. Like Article");
                Console.WriteLine("3. Dislike Article");
                Console.WriteLine("4. Back");
                Console.WriteLine("5. Logout");

                int choice = InputHelper.GetInt("Choose option: ", 1, 5);

                switch (choice)
                {
                    case 1:
                        ToggleSaveResponseDto saved = await articleService.SaveArticleForUserAsync(article.ArticleId);
                        Console.WriteLine(saved.Message);
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case 2:
                        await articleService.AddArticleReactionAsync(new ArticleReactionRequestDto
                        {
                            ArticleId = article.ArticleId,
                            ArticleReaction = ReactionEnum.Like
                        });
                        Console.WriteLine("You liked the article!");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case 3:
                        await articleService.AddArticleReactionAsync(new ArticleReactionRequestDto
                        {
                            ArticleId = article.ArticleId,
                            ArticleReaction = ReactionEnum.Dislike
                        });
                        Console.WriteLine("You disliked the article!");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case 4:
                        return;

                    case 5:
                        Environment.Exit(0);
                        break;
                }
            }
        }

    }
}
