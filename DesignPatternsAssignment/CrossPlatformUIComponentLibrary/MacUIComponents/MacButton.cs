using CrossPlatformUIComponentLibrary.Interfaces;

namespace CrossPlatformUIComponentLibrary.MacUIComponents
{
    public class MacButton : IButton
    {
        public void Render()
        {
            Console.WriteLine("Mac Button Rendered");
        }
    }
}
