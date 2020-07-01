using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

namespace Pretend
{
    public interface IWindow
    {
        void OnUpdate();
    }

    public class Window : IWindow
    {
        private INativeWindow _window;

        public Window()
        {
            _window = new NativeWindow(NativeWindowSettings.Default);
        }

        public void OnUpdate()
        {
        }
    }
}
