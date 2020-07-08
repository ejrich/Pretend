using System;
using SDL2;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend
{
    public interface IWindow
    {
        void Init();
        void OnUpdate();
        void Close();
        bool IsClosing { get; }
    }

    public class Window : IWindow
    {
        private IntPtr _window;

        public bool IsClosing { get; private set; }

        public void Init()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _window = SDL.SDL_CreateWindow("Hello", 0, 0, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            var context = SDL.SDL_GL_CreateContext(_window);

            GL.LoadBindings(new SDLContext());

            SDL.SDL_GL_SetSwapInterval(1);
        }

        public void OnUpdate()
        {
            while (SDL.SDL_PollEvent(out var evnt) != 0)
            {
                if (evnt.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    IsClosing = true;
                    return;
                }
            }

            GL.ClearColor(0.2f, 0.4f, 0.4f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SDL.SDL_GL_SwapWindow(_window);
        }

        public void Close()
        {
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }

    public class SDLContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }
}
