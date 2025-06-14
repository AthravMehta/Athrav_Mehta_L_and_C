using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;
using NewsAggregationConsole.Enums;

namespace NewsAggregationConsole
{
    class Program
    {
        private static ApiService _apiService;
        private static AuthService _authService;
        private static string _baseApiUrl = "https://localhost:7112/";
        private static UserDto _currentUser;

        static async Task Main(string[] args)
        {
            _apiService = new ApiService(_baseApiUrl);
            _authService = new AuthService(_apiService);
            await MainMenu();
        }

        private static async Task MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to News App");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Sign Up");
                Console.WriteLine("3. Exit");

                var choice = InputHelper.GetInt("Choose option: ", 1, 3);

                switch (choice)
                {
                    case 1:
                        await LoginFlow();
                        break;
                    case 2:
                        await SignUpFlow();
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static async Task LoginFlow()
        {
            Console.Clear();
            var username = InputHelper.GetString("Username: ");
            var password = InputHelper.GetPassword("Password: ");

            try
            {
                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };
                _currentUser = await _authService.LoginAsync(loginDto);
                if (_currentUser == null)
                {
                    InputHelper.ShowError("Invalid credentials");
                    return;
                }

                if (_currentUser.RoleId == RoleEnum.Admin)
                    await AdminFlow();
                else
                    await UserFlow();
            }
            catch (Exception ex)
            {
                InputHelper.ShowError(ex.Message);
            }
        }

        private static async Task SignUpFlow()
        {
            Console.Clear();
            var username = InputHelper.GetString("Username: ");
            var password = InputHelper.GetPassword("Password: ");
            var email = InputHelper.GetEmail("Email: ");

            try
            {
                var registerDto = new RegisterDto
                {
                    Username = username,
                    Password = password,
                    Email = email
                };
                _currentUser = await _authService.SignUpAsync(registerDto);
                if (_currentUser == null)
                {
                    InputHelper.ShowError("Invalid credentials");
                    return;
                }
                await UserFlow();
            }
            catch (Exception ex)
            {
                InputHelper.ShowError(ex.Message);
            }
        }

        private static async Task Logout()
        {
            try
            {
                _authService.LogoutAsync();
                _currentUser = null;
            }
            catch (Exception ex)
            {
                InputHelper.ShowError(ex.Message);
            }
        }

        private static async Task AdminFlow()
        {
            while (true)
            {
                Console.Clear();
                //AdminHeader();
                Console.WriteLine("1. List External Servers");
                Console.WriteLine("2. External Server Details");
                Console.WriteLine("3. Update External Server");
                Console.WriteLine("4. Add News Category");
                Console.WriteLine("5. Logout");

                var choice = InputHelper.GetInt("Choose option: ", 1, 5);

                switch (choice)
                {
                    case 1:
                        //await ListExternalServers();
                        break;
                    case 2:
                        //await ViewServerDetails();
                        break;
                    case 3:
                        //await UpdateServer();
                        break;
                    case 4:
                        //await AddCategory();
                        break;
                    case 5:
                        await Logout();
                        return;
                }
            }
        }

        private static async Task UserFlow()
        {
            while (true)
            {
                Console.Clear();
                //UserHeader();
                Console.WriteLine("1. Headlines");
                Console.WriteLine("2. Saved Articles");
                Console.WriteLine("3. Search");
                Console.WriteLine("4. Notifications");
                Console.WriteLine("5. Logout");

                var choice = InputHelper.GetInt("Choose option: ", 1, 5);

                switch (choice)
                {
                    case 1:
                        //await HeadlinesMenu();
                        break;
                    case 2:
                        //await SavedArticlesMenu();
                        break;
                    case 3:
                        //await SearchMenu();
                        break;
                    case 4:
                        //await NotificationsMenu();
                        break;
                    case 5:

                        _currentUser = null;
                        return;
                }
            }
        }
    }
}
