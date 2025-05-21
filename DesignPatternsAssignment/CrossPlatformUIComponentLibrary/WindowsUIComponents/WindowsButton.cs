using CrossPlatformUIComponentLibrary.Interfaces;

namespace CrossPlatformUIComponentLibrary.MacUIComponents
{
    public class WindowsButton : IButton
    {
        public void Render()
        {
            Console.WriteLine("Windows Button Rendered");
        }
    }
}
