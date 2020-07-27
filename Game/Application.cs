using Pretend;

namespace Game
{
    public class Application : IApplication
    {
        
    }

    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Game";
    }
}
