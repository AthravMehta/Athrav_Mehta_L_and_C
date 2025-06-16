using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Helpers;

namespace NewsAggregationConsole.Flows
{
    public static class NotificationsFlow
    {
        public static async Task Run(UserDto currentUser, ApiService apiService)
        {
            var notificationService = new NotificationService(apiService);

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
                        //await ConfigureNotifications(notificationService, currentUser);
                        break;
                    case 3:
                        return;
                    case 4:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static async Task ViewNotifications(NotificationService notificationService, UserDto user)
        {
            var notifications = await notificationService.GetUnviewedNotificationsAsync(user.Id);
            DisplayHelper.ShowNotifications(notifications);

            if (notifications.Any())
            {
                var ids = notifications.Select(n => n.Id).ToList();
                //await notificationService.MarkNotificationsAsViewedAsync(ids);
            }

            InputHelper.GetString("Press Enter to continue...");
        }

        //private static async Task ConfigureNotifications(NotificationService notificationService, UserDto user)
        //{
        //    while (true)
        //    {
        //        var config = await notificationService.GetNotificationConfigAsync(user.Id);
        //        DisplayHelper.ShowNotificationConfig(config);

        //        Console.WriteLine("Select option to toggle/enter keyword, 6: Back, 7: Logout");
        //        var choice = InputHelper.GetInt("Option: ", 1, 7);

        //        if (choice >= 1 && choice <= 4)
        //        {
        //            await notificationService.ToggleCategoryAsync(user.Id, config.Categories[choice - 1].CategoryName);
        //        }
        //        else if (choice == 5)
        //        {
        //            var keyword = InputHelper.GetString("Enter keyword to add: ");
        //            await notificationService.AddKeywordAsync(user.Id, keyword);
        //        }
        //        else if (choice == 6)
        //        {
        //            return;
        //        }
        //        else if (choice == 7)
        //        {
        //            Environment.Exit(0);
        //        }
        //    }
        //}
    
    }
}
