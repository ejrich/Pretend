using System;
using GLFW;
// using OpenToolkit.Windowing.Common;
// using OpenToolkit.Windowing.Desktop;
// using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Pretend
{
    public interface IWindow
    {
        void OnUpdate();
    }

    public class Window : IWindow
    {
        // private INativeWindow _window;

        public unsafe Window()
        {
            var a = Glfw.Init();

            // GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            // GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 6);

            // GLFW.WindowHint(WindowHintBool.Visible, true);
            var window = Glfw.CreateWindow(800, 600, "Hello world", Monitor.None, GLFW.Window.None);
            // GLFW.MakeContextCurrent(window);

            // var settings = NativeWindowSettings.Default;
            // settings.APIVersion = new Version(4, 0);

            // _window = new NativeWindow(settings);
        }

        public void OnUpdate()
        {
        }
    }
}
