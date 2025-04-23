using System.Text.RegularExpressions;

namespace ATMMachine.Utils
{
    public static class Validation
    {
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Basic email regex pattern : Must include 1 '@' and can include mulitple '.'
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Minimum 6 characters, at least one letter and one number
            var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d).{6,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        public static bool ValidatePin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 4)
                return false;

            return Regex.IsMatch(pin, @"^\d{4}$");
        }
    }
}
