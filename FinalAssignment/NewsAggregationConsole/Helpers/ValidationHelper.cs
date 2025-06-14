using System.Text.RegularExpressions;

namespace NewsAggregationConsole.Helpers
{
    public static class ValidationHelper
    {
        public static bool validatePassword(string password)
        {
            if(string.IsNullOrEmpty(password)) return false;
            return IsValidPassword(password);
        }

        public static bool validateEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return IsValidEmail(email);
        }

        private static bool IsValidPassword(string password)
        {
            // At least 8 chars, 1 uppercase, 1 lowercase, 1 special char
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$");
            return regex.IsMatch(password);
        }
        private static bool IsValidEmail(string email)
        {
            // Simple email regex pattern (@)(.)
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
