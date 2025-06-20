using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Helpers;

namespace NewsAggregationConsole.Flows
{
    public static class UserFlowManager
    {
        public static async Task Run(UserReadDto currentUser, ApiService apiService)
        {
            while (true)
            {
                Console.Clear();
                DisplayHelper.ShowHeader(currentUser);
                Console.WriteLine("1. Headlines");
                Console.WriteLine("2. Saved Articles");
                Console.WriteLine("3. Search");
                Console.WriteLine("4. Notifications");
                Console.WriteLine("5. Logout");

                var choice = InputHelper.GetInt("Choose option: ", 1, 5);

                switch (choice)
                {
                    case 1:
                        await HeadlinesFlow.Run(currentUser, apiService);
                        break;
                    case 2:
                        await SavedArticlesFlow.Run(currentUser, apiService);
                        break;
                    case 3:
                        await SearchFlow.Run(currentUser, apiService);
                        break;
                    case 4:
                        await NotificationsFlow.Run(currentUser, apiService);
                        break;
                    case 5:
                        return;
                }
            }
        }
    }
}
