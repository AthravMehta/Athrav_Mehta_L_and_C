using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Helpers;

namespace NewsAggregationConsole.Flows
{
    public static class AdminFlowManager
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            var categoryService = new CategoryService(apiService);
            var keywordService = new KeywordService(apiService);
            var externalServerService = new ExternalServerService(apiService);

            while (true)
            {
                Console.Clear();
                DisplayHelper.ShowHeader(currentUser, true);
                Console.WriteLine("1. List External Servers");
                Console.WriteLine("2. External Server Details");
                Console.WriteLine("3. Update External Server");
                Console.WriteLine("4. Add News Category");
                Console.WriteLine("5. List All Category");
                Console.WriteLine("6. Logout");

                var choice = InputHelper.GetInt("Choose option: ", 1, 6);

                switch (choice)
                {
                    case 1:
                        await ListExternalServers(externalServerService);
                        break;
                    case 2:
                        await ViewServerDetails(externalServerService);
                        break;
                    case 3:
                        await UpdateServer(externalServerService);
                        break;
                    case 4:
                        await AddCategory(categoryService, keywordService, currentUser);
                        break;
                    case 5:
                        await ListCategory(categoryService, keywordService, currentUser);
                        break;
                    case 6:
                        return;
                }
            }
        }
        private static async Task ListExternalServers(ExternalServerService service)
        {
            var servers = await service.GetAllExternalServerAsync();
            DisplayHelper.ShowExternalServers(servers);
            InputHelper.GetString("Press Enter to continue...");
        }

        private static async Task ViewServerDetails(ExternalServerService service)
        {
            var servers = await service.GetAllExternalServerAsync();
            DisplayHelper.ShowExternalServers(servers);

            var idStr = InputHelper.GetString("Enter Server ID: ");
            if (!int.TryParse(idStr, out var id))
            {
                InputHelper.ShowError("Invalid int format.");
                return;
            }

            var server = await service.GetExternalServerByIdAsync(id);
            if (server == null)
            {
                InputHelper.ShowError("Server not found.");
                return;
            }

            DisplayHelper.ShowExternalServerDetails(server);
            InputHelper.GetString("Press Enter to continue...");
        }

        private static async Task UpdateServer(ExternalServerService service)
        {
            var servers = await service.GetAllExternalServerAsync();
            DisplayHelper.ShowExternalServers(servers);

            var idStr = InputHelper.GetString("Enter Server ID to update: ");
            if (!int.TryParse(idStr, out var id))
            {
                InputHelper.ShowError("Invalid int format.");
                return;
            }

            var server = await service.GetExternalServerByIdAsync(id);
            if (server == null)
            {
                InputHelper.ShowError("Server not found.");
                return;
            }

            DisplayHelper.ShowUpdateExternalServerPrompt(server);

            var newApiKey = InputHelper.GetString($"API Key Hash [{server.ApiKeyHash}]: ", true);
            var newStatusStr = InputHelper.GetString($"Is Active (yes/no) [{(server.IsActive ? "yes" : "no")}]: ", true);

            if (!string.IsNullOrWhiteSpace(newApiKey))
                server.ApiKeyHash = newApiKey;
            if (!string.IsNullOrWhiteSpace(newStatusStr))
                server.IsActive = newStatusStr.Trim().ToLower().StartsWith("y");

            await service.UpdateExternalServerAsync(server.ExternalServerId!.Value, server);
            InputHelper.ShowSuccess("External server updated successfully!");
        }

        private static async Task AddCategory(CategoryService categoryService, KeywordService keywordService, UserReadDto currentUser)
        {
            DisplayHelper.ShowHeader(currentUser, true);
            Console.WriteLine("Add New News Category");
            Console.WriteLine("---------------------");

            var name = InputHelper.GetString("Enter category name: ");

            var keywordsInput = InputHelper.GetString("Enter keywords for this category (comma separated): ");
            var keywordsList = keywordsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(k => k.Trim())
                                           .Where(k => !string.IsNullOrWhiteSpace(k))
                                           .ToList();

            try
            {
                var addedCategory = await categoryService.AddCategoryAsync(name);

                InputHelper.ShowSuccess($"Category '{addedCategory.Name}' added successfully with ID {addedCategory.CategoryId}!", true);

                if (keywordsList.Any())
                {
                    var keywordsToAdd = keywordsList.Select(keyword => new KeywordDto
                    {
                        Keyword = keyword,
                        CategoryId = addedCategory.CategoryId!.Value,
                    }).ToList();

                    var keywordsResult = await keywordService.AddKeywordsAsync(keywordsToAdd);

                    if (keywordsResult.IsSuccess)
                    {
                        InputHelper.ShowSuccess(keywordsResult.Message);
                    }
                    else
                    {
                        InputHelper.ShowError($"Failed to add keywords: {keywordsResult.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No keywords entered. Category added without keywords.");
                }
            }
            catch (Exception ex)
            {
                InputHelper.ShowError($"Failed to add category or keywords: {ex.Message}");
            }
        }

        private static async Task ListCategory(CategoryService categoryService, KeywordService keywordService, UserReadDto currentUser)
        {
            while (true)
            {
                var categories = await categoryService.GetAllCategoryAsync();
                var keywords = await keywordService.GetAllKeywordsAsync();

                var categoriesWithKeywords = categories
                    .Where(cat => cat.CategoryId.HasValue && cat.IsHidden.HasValue)
                    .Select(cat => new CategoryWithKeywordsDto
                    {
                        CategoryId = cat.CategoryId.Value,
                        Name = cat.Name ?? string.Empty,
                        IsHidden = cat.IsHidden.Value,
                        HideReason = cat.HideReason ?? string.Empty,
                        Keywords = keywords.Where(k => k.CategoryId == cat.CategoryId).ToList()
                    })
                    .ToList();


                DisplayHelper.DisplayAllCategoriesWithKeywords(categoriesWithKeywords);

                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Toggle Hide/Unhide Category");
                Console.WriteLine("2. Toggle Hide/Unhide Keyword");
                Console.WriteLine("3. Back");

                int choice = InputHelper.GetInt("Choose option: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        await DisplayHelper.HandleCategoryHideUnhideAsync(categoryService, categoriesWithKeywords);
                        break;
                    case 2:
                        await DisplayHelper.HandleKeywordHideUnhideAsync(keywordService, categoriesWithKeywords);
                        break;
                    case 3:
                        return;
                }
            }
        }
    }
}
