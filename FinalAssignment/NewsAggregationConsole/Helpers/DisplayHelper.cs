using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Helpers
{
    public static class DisplayHelper
    {
        public static void ShowHeader(UserDto user, bool isAdmin = false)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Welcome, {user.Username}{(isAdmin ? " (Admin)" : "")}!");
            Console.WriteLine($"Date: {DateTime.Now:dd-MMM-yyyy}  Time: {DateTime.Now:hh:mm tt}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 40));
        }

        public static void DisplayAllCategoryAdmin(List<CategoryDto> categoryDtos)
        {
            Console.Clear();
            Console.WriteLine("All News Categories: ");
            Console.WriteLine();
            foreach (CategoryDto categoryDto in categoryDtos)
            {
                Console.WriteLine(categoryDto.Name);
            }
            Console.ReadKey();
        }

        public static void ShowExternalServers(List<ExternalServerDto> servers)
        {
            Console.Clear();
            Console.WriteLine("External Servers List:");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("ID                                   | Name                | Active | Last Accessed");
            Console.WriteLine("--------------------------------------------------------------------------------");

            foreach (var s in servers)
            {
                Console.WriteLine($"{s.Id} | {s.ServerName,-20} | {(s.IsActive ? "Yes" : "No"),-6}");
            }
            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        public static void ShowExternalServerDetails(ExternalServerDto server)
        {
            Console.Clear();
            Console.WriteLine("External Server Details:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"ID           : {server.Id}");
            Console.WriteLine($"Name         : {server.ServerName}");
            Console.WriteLine($"Base URL     : {server.BaseUrl}");
            Console.WriteLine($"Active       : {(server.IsActive ? "Yes" : "No")}");
            Console.WriteLine($"API Key Hash : {server.ApiKeyHash}");
            //Console.WriteLine($"Last Accessed: {server.LastAccessed:yyyy-MM-dd HH:mm}");
            Console.WriteLine("------------------------------------------------");
        }
        public static void ShowUpdateExternalServerPrompt(ExternalServerDto server)
        {
            Console.Clear();
            Console.WriteLine("Update External Server:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"ID           : {server.Id}");
            Console.WriteLine($"Name         : {server.ServerName}");
            Console.WriteLine($"Base URL     : {server.BaseUrl}");
            Console.WriteLine($"Active       : {(server.IsActive ? "Yes" : "No")}");
            Console.WriteLine($"API Key Hash : {server.ApiKeyHash}");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Leave input blank to keep the current value.");
        }

        public static void ShowNotifications(List<NotificationDto> notifications)
        {
            Console.Clear();
            Console.WriteLine("Unviewed Notifications:");
            if (!notifications.Any())
            {
                Console.WriteLine("No new notifications.");
                return;
            }
            foreach (var n in notifications)
            {
                Console.WriteLine($"[{n.Id}] {n.IsRead} (Article: {n.ArticleId}, Date: {n.SentDateTime:yyyy-MM-dd HH:mm})");
            }
            Console.WriteLine();
        }

        //public static void ShowNotificationConfig(NotificationConfigurationDto config)
        //{
        //    Console.Clear();
        //    Console.WriteLine("Notification Configuration:");
        //    int i = 1;
        //    foreach (var cat in config.Categories)
        //    {
        //        Console.WriteLine($"{i++}. {cat.CategoryName} - {(cat.IsEnabled ? "Enabled" : "Disabled")}");
        //    }
        //    Console.WriteLine($"{i++}. Keywords - {(config.KeywordsEnabled ? "Enabled" : "Disabled")}");
        //    Console.WriteLine();
        //}
    }
}
