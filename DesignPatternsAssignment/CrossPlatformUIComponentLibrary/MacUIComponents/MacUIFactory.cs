using CrossPlatformUIComponentLibrary.Interfaces;

namespace CrossPlatformUIComponentLibrary.MacUIComponents
{
    public class MacUIFactory : IUIFactory
    {
        public IButton CreateButton() => new MacButton();

        public ICheckbox CreateCheckbox() => new MacCheckbox();

        public ITextField CreateTextField() => new MacTextField();
    }
}
