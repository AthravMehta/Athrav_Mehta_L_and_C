using ATMMachine.Constants;

namespace ATMMachine.Utils
{
    public static class InputHelper
    {
        public static string ReadNonEmptyString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Input cannot be empty. Try again or type 'q' to quit.");

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException("User chose to quit.");

            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static int ReadInt(string prompt)
        {
            int result;
            bool isValid;
            do
            {
                Console.Write(prompt);
                isValid = int.TryParse(Console.ReadLine(), out result);
                if (!isValid)
                {
                    Console.WriteLine(ErrorConstant.InvalidNumber);
                }
            }
            while (!isValid);

            return result;
        }

        public static decimal ReadDecimal(string prompt)
        {
            decimal result;
            bool isValid;
            do
            {
                Console.Write(prompt);
                isValid = decimal.TryParse(Console.ReadLine(), out result);
                if (!isValid || result <= 0)
                {
                    Console.WriteLine(ErrorConstant.AmountGreaterThanZero);
                    isValid = false;
                }
            }
            while (!isValid);

            return result;
        }

        public static int ReadPin(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException("User chose to quit.");

                if (!Validation.ValidatePin(input))
                {
                    Console.WriteLine("PIN must be exactly 4 digits. Try again or type 'q' to quit.");
                }
                else
                {
                    return int.Parse(input);
                }

            } while (true);
        }
        
        public static string ReadEmail(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Input cannot be empty. Try again or type 'q' to quit.");

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException("User chose to quit.");

                if (!Validation.ValidateEmail(input))
                    Console.WriteLine("Invalid email format. Try again or type 'q' to quit.");

            } while (!Validation.ValidateEmail(input));

            return input;
        }

        public static string ReadPassword(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Input cannot be empty. Try again or type 'q' to quit.");

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    throw new OperationCanceledException("User chose to quit.");

                if (!Validation.ValidatePassword(input))
                    Console.WriteLine("Password must be at least 6 characters and contain letters and numbers. Try again or type 'q' to quit.");
            } while (!Validation.ValidatePassword(input));

            return input;
        }
    }
}

