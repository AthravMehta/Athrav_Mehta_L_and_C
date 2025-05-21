using CrossPlatformUIComponentLibrary.Interfaces;
using CrossPlatformUIComponentLibrary.MacUIComponents;

namespace CrossPlatformUIComponentLibrary.WindowsUIComponents
{
    public class WindowsUIFactory : IUIFactory
    {
        public IButton CreateButton() => new WindowsButton();

        public ICheckbox CreateCheckbox() => new WindowsCheckbox();

        public ITextField CreateTextField() => new WindowsTextField();
    }
}
