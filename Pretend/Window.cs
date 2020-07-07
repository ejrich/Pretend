using System;
using SDL2;

namespace Pretend
{
    public interface IWindow
    {
        void OnUpdate();
    }

    public class Window : IWindow
    {
        private IntPtr _window;

        public unsafe Window()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _window = SDL.SDL_CreateWindow("Hello", 0, 0, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }

        public void OnUpdate()
        {
        }
    }
}
