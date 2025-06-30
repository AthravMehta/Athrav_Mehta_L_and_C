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
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            var query = new ArticleQueryDto
            {
                StartDate = today,
                EndDate = today,
                CategoryId = null
            };
            var articles = await articleService.GetFilteredArticlesAsync(query);
            await ArticleFlowHelper.ShowArticlesPage(articles, user, articleService, allowSave: true);
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
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                CategoryId = selectedCategory.CategoryId
            };

            List<ArticleDto> articles;
            if (selectedCategory.CategoryId == 0)
            {
                query.CategoryId = null;
            }
            articles = await articleService.GetFilteredArticlesAsync(query);
            await ArticleFlowHelper.ShowArticlesPage(articles, user, articleService, allowSave: true);
        }
    }
}
