namespace NewsAggregationConsole.Helpers
{
    public static class InputHelper
    {
        public static string GetString(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (allowEmpty || !string.IsNullOrWhiteSpace(input)) return input;
                ShowError("Field cannot be empty");
            }
        }
        public static string GetPassword(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if ((allowEmpty || !string.IsNullOrWhiteSpace(input)) && ValidationHelper.validatePassword(input)) return input;
                ShowError("Password cannot be empty");
            }
        }
        public static string GetEmail(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if ((allowEmpty || !string.IsNullOrWhiteSpace(input)) && ValidationHelper.validateEmail(input)) return input;
                ShowError("Email cannot be empty");
            }
        }

        public static int GetInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var result) && result >= min && result <= max)
                    return result;
                ShowError($"Please enter a valid number between {min}-{max}");
            }
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.ReadKey();
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
