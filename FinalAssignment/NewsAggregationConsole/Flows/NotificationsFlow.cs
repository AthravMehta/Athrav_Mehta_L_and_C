using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Helpers;

namespace NewsAggregationConsole.Flows
{
    public static class NotificationsFlow
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            var notificationService = new NotificationService(apiService);
            var notificationConfigurationService = new NotificationConfigurationService(apiService);

            while (true)
            {
                DisplayHelper.ShowHeader(currentUser);
                Console.WriteLine("Notifications Menu");
                Console.WriteLine("-------------------");
                Console.WriteLine("1. View Notifications");
                Console.WriteLine("2. Configure Notifications");
                Console.WriteLine("3. Back");
                Console.WriteLine("4. Logout");

                var choice = InputHelper.GetInt("Choose option: ", 1, 4);

                switch (choice)
                {
                    case 1:
                        await ViewNotifications(notificationService, currentUser);
                        break;
                    case 2:
                        await ConfigureNotifications(notificationConfigurationService, currentUser);
                        break;
                    case 3:
                        return;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static async Task ViewNotifications(NotificationService notificationService, UserReadDto user)
        {
            var notifications = await notificationService.GetUnviewedNotificationsAsync();
            DisplayHelper.ShowNotifications(notifications);

            if (notifications.Any())
            {
                var ids = notifications.Select(n => n.UserId).ToList();
            }

            InputHelper.GetString("Press Enter to continue...");
        }

        private static async Task ConfigureNotifications(NotificationConfigurationService notificationConfigurationService, UserReadDto user)
        {
            while (true)
            {
                var configs = await notificationConfigurationService.GetUserNotificationConfigAsync();

                if (configs == null || !configs.Any())
                {
                    Console.WriteLine("No notification configuration found for user.");
                    return;
                }

                DisplayHelper.ShowNotificationConfig(configs);

                Console.WriteLine("\nSelect an option:");

                for (int i = 0; i < configs.Count; i++)
                {
                    var config = configs[i];
                    string status = config.IsEnabled ? "[Enabled]" : "[Disabled]";
                    Console.WriteLine($"{i + 1}. Toggle Category: {config.Category!.Name} {status}");
                }

                int addKeywordOption = configs.Count + 1;
                int backOption = configs.Count + 2;
                int logoutOption = configs.Count + 3;

                Console.WriteLine($"{addKeywordOption}. Add Keyword");
                Console.WriteLine($"{backOption}. Back");
                Console.WriteLine($"{logoutOption}. Logout");

                int choice = InputHelper.GetInt("Option: ", 1, logoutOption);

                if (choice >= 1 && choice <= configs.Count)
                {
                    var selectedCategoryConfig = configs[choice - 1];
                    selectedCategoryConfig.IsEnabled = !selectedCategoryConfig.IsEnabled;

                    configs[choice -1].IsEnabled = (await notificationConfigurationService.ToggleCategoryAsync(selectedCategoryConfig.UserNotificationConfigurationId, selectedCategoryConfig)).IsEnabled;
                    Console.WriteLine("Category Configuration Changed!");
                    InputHelper.GetString("Press Enter to continue...");
                }
                else if (choice == addKeywordOption)
                {
                    var keyword = InputHelper.GetString("Enter keyword to add: ");

                    Console.WriteLine("Select category to associate the keyword with:");

                    for (int i = 0; i < configs.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {configs[i].Category!.Name}");
                    }

                    int categoryChoice = InputHelper.GetInt("Category option: ", 1, configs.Count);
                    var selectedCategory = configs[categoryChoice - 1];

                    var userKeywordDto = new UserKeywordDto
                    {
                        UserId = user.UserId,
                        Keyword = keyword,
                        CategoryId = selectedCategory.CategoryId,
                    };

                    await notificationConfigurationService.AddKeywordAsync(userKeywordDto);
                }
                else if (choice == backOption)
                {
                    return;
                }
                else if (choice == logoutOption)
                {
                    Environment.Exit(0);
                }
            }
        }


    }
}
