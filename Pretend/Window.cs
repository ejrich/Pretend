using System;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using Pretend.Events;
using SDL2;

namespace Pretend
{
    public interface IWindow
    {
        void Init();
        void OnUpdate();
        void Close();
    }

    public class Window : IWindow
    {
        private IntPtr _window;
        private readonly IEventDispatcher _eventDispatcher;

        public Window(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public void Init()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _window = SDL.SDL_CreateWindow("Hello", 0, 0, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            var context = SDL.SDL_GL_CreateContext(_window);

            GL.LoadBindings(new SDLContext());

            SDL.SDL_GL_SetSwapInterval(1);
        }

        public void OnUpdate()
        {
            while (SDL.SDL_PollEvent(out var evnt) != 0) HandleEvent(evnt);

            GL.ClearColor(0.2f, 0.4f, 0.4f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SDL.SDL_GL_SwapWindow(_window);
        }

        public void Close()
        {
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }

        private void HandleEvent(SDL.SDL_Event evnt)
        {
            switch (evnt.type)
            {
                // Window Events
                case SDL.SDL_EventType.SDL_QUIT:
                    _eventDispatcher.DispatchEvent(new WindowCloseEvent());
                    break;
                case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    var windowEvent = evnt.window;
                    switch (windowEvent.windowEvent)
                    {
                        case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                            _eventDispatcher.DispatchEvent(new WindowResizeEvent { Width = windowEvent.data1, Height = windowEvent.data2 });
                            break;
                    }
                    break;

                // Mouse Events
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    _eventDispatcher.DispatchEvent(new MouseMovedEvent { X = evnt.motion.x, Y = evnt.motion.y });
                    break;
                case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                    _eventDispatcher.DispatchEvent(new MouseScrollEvent { XOffset = evnt.wheel.x, YOffset = evnt.wheel.y });
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    _eventDispatcher.DispatchEvent(new MouseButtonPressedEvent { Button = evnt.button.button });
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    _eventDispatcher.DispatchEvent(new MouseButtonReleasedEvent { Button = evnt.button.button });
                    break;

                // Key Events
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    _eventDispatcher.DispatchEvent(new KeyPressedEvent { KeyCode = (int) evnt.key.keysym.sym });
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    _eventDispatcher.DispatchEvent(new KeyReleasedEvent { KeyCode = (int) evnt.key.keysym.sym });
                    break;
            };
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
