namespace CrossPlatformUIComponentLibrary.Interfaces
{
    public interface IUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
        ITextField CreateTextField();
    }
}
