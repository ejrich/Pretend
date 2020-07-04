using System.Text;
using System.Runtime.InteropServices;
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

        [DllImport("libglfw.so.3.3")]
        public static extern int glfwInit();

        [DllImport("libglfw.so.3.3")]
        private static extern int glfwCreateWindow(int width, int height, byte[] title, Monitor monitor, GLFW.Window share);

        public unsafe Window()
        {
            // NativeLibrary.SetDllImportResolver(typeof(Glfw).Assembly, (name, assembly, path) =>
            // {
            //     if (name != Glfw.LIBRARY)
            //     {
            //         return IntPtr.Zero;
            //     }

            //     if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //     {
            //         return NativeLibrary.Load("libglfw.so.3.3", assembly, path);
            //     }

            //     if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //     {
            //         return NativeLibrary.Load("libglfw.3.dylib", assembly, path);
            //     }

            //     if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //     {
            //         if (IntPtr.Size == 8)
            //         {
            //             return NativeLibrary.Load("glfw3-x64.dll", assembly, path);
            //         }

            //         return NativeLibrary.Load("glfw3-x86.dll", assembly, path);
            //     }

            //     return IntPtr.Zero;
            // });
            // var a = Glfw.Init();
            var a = glfwInit();

            // GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4)
            // GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 6);

            // GLFW.WindowHint(WindowHintBool.Visible, true);
            // var window = Glfw.CreateWindow(800, 600, "Hello world", Monitor.None, GLFW.Window.None);
            var window = glfwCreateWindow(800, 600, Encoding.UTF8.GetBytes("Hello world"), Monitor.None, GLFW.Window.None);
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
