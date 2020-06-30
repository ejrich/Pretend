using OpenToolkit;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

namespace Pretend
{
    public interface IWindow
    {
        void Show();
    }

    public class Window : IWindow
    {
        private INativeWindow _window;

        public Window()
        {
            // _window = new GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default);
            _window = new NativeWindow(NativeWindowSettings.Default);
        }

        public void Show()
        {
            // _window.Run();
        }
    }
}
