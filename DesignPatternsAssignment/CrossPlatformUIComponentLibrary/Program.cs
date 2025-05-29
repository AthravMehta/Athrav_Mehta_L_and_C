// Interface IOperatingSystem -> (method) GetOS()
// Class to resolve the OS

// class WindowsOS implements IOperatingSystem -> (method) GetOS();
// class MacOS implements IOperatingSystem -> (method) GetOS();

// Dono Classes mai UI components bhi rkhne hai -> Buttons, Checkboxes, and TextFields.

using CrossPlatformUIComponentLibrary.Interfaces;
using CrossPlatformUIComponentLibrary.MacUIComponents;
using CrossPlatformUIComponentLibrary.WindowsUIComponents;

namespace CrossPlatformUIComponentLibrary
{
    class Program
    {
        public static void Main(string[] args)
        {
            IUIFactory uiFactory;

            Console.WriteLine("Choose OS: 1. Windows  2. Mac");
            string choice = Console.ReadLine();

            uiFactory = choice switch
            {
                "1" => new WindowsUIFactory(),
                "2" => new MacUIFactory(),
                _ => throw new ArgumentException("Invalid platform choice")
            };

            var button = uiFactory.CreateButton();
            var checkbox = uiFactory.CreateCheckbox();
            var textField = uiFactory.CreateTextField();

            button.Render();
            checkbox.Render();
            textField.Render();
        }
    }
}