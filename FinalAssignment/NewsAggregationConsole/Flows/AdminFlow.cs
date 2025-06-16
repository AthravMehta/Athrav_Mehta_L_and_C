using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Helpers;

namespace NewsAggregationConsole.Flows
{
    public static class AdminFlowManager
    {
        public static async Task Run(UserDto currentUser, ApiService apiService)
        {
            var categoryService = new CategoryService(apiService);
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
                        await AddCategory(categoryService, currentUser);
                        break;
                    case 5:
                        await ListCategory(categoryService, currentUser);
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
            if (!Guid.TryParse(idStr, out var id))
            {
                InputHelper.ShowError("Invalid GUID format.");
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
            if (!Guid.TryParse(idStr, out var id))
            {
                InputHelper.ShowError("Invalid GUID format.");
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

            await service.UpdateExternalServerAsync(server.Id.Value, server);
            InputHelper.ShowSuccess("External server updated successfully!");
        }
    
        private static async Task AddCategory(CategoryService categoryService, UserDto currentUser)
        {
            DisplayHelper.ShowHeader(currentUser, true);
            Console.WriteLine("Add New News Category");
            Console.WriteLine("---------------------");
            var name = InputHelper.GetString("Enter category name: ");
            try
            {
                await categoryService.AddCategoryAsync(name);
                InputHelper.ShowSuccess("Category added successfully!");
            }
            catch (Exception ex)
            {
                InputHelper.ShowError($"Failed to add category: {ex.Message}");
            }
        }

        private static async Task ListCategory(CategoryService categoryService, UserDto currentUser)
        {
            DisplayHelper.ShowHeader(currentUser, true);
            Console.WriteLine("All News Category");
            Console.WriteLine("---------------------");
            try
            {
                var categoryDtos = await categoryService.GetAllCategoryAsync();
                DisplayHelper.DisplayAllCategoryAdmin(categoryDtos);
            }
            catch (Exception ex)
            {
                InputHelper.ShowError($"Failed to List category: {ex.Message}");
            }
        }

    }
}
