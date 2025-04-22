using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMMachine.Utils
{
    public class Menu
    {
        public static void ShowMainMenu()
        {
            Console.WriteLine("\nATM Machine:");
            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit\n");
        }
        public static void ShowAtmMenu()
        {
            Console.WriteLine("\nATM Operations:");
            Console.WriteLine("1. Withdraw");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Check Balance");
            Console.WriteLine("4. Logout");
        }
    }
}
