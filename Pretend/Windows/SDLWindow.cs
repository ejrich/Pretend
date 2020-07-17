using System;
using Pretend.Events;
using Pretend.Graphics;
using SDL2;

namespace Pretend.Windows
{
    public class SDLWindow : IWindow
    {
        private IntPtr _window;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IGraphicsContext _context;

        private ulong lastTime;

        public SDLWindow(IEventDispatcher eventDispatcher, IGraphicsContext context)
        {
            _eventDispatcher = eventDispatcher;
            _context = context;
        }

        public void Init(WindowAttributes attributes)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _window = SDL.SDL_CreateWindow(attributes.Title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                attributes.Width, attributes.Height, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            _context.CreateContext(_window);

            SDL.SDL_GL_SetSwapInterval(1);
        }

        public float GetTimestep()
        {
            var now = SDL.SDL_GetPerformanceCounter();
            if (lastTime == 0) lastTime = now;

            var step = (now - lastTime) / (float) SDL.SDL_GetPerformanceFrequency();
            lastTime = now;

            return step;
        }

        public void OnUpdate()
        {
            while (SDL.SDL_PollEvent(out var evnt) != 0) HandleEvent(evnt);

            _context.UpdateWindow();
        }

        public void Close()
        {
            _context.DeleteContext();

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
                    _eventDispatcher.DispatchEvent(new MouseButtonPressedEvent { Button = (MouseButton) evnt.button.button });
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    _eventDispatcher.DispatchEvent(new MouseButtonReleasedEvent { Button = (MouseButton) evnt.button.button });
                    break;

                // Key Events
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    _eventDispatcher.DispatchEvent(new KeyPressedEvent { KeyCode = (KeyCode) evnt.key.keysym.sym });
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    _eventDispatcher.DispatchEvent(new KeyReleasedEvent { KeyCode = (KeyCode) evnt.key.keysym.sym });
                    break;
            };
        }
    }
}
