using System;
using System.Threading;
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

        private ulong _lastTime;
        private float _performanceFrequency;

        private uint _maxFps;
        private float _maxTimestep;

        public SDLWindow(IEventDispatcher eventDispatcher, IGraphicsContext context)
        {
            _eventDispatcher = eventDispatcher;
            _context = context;
        }

        public void Init(string title, ISettingsManager settings)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                settings.Resolution.X, settings.Resolution.Y, GetWindowFlags(settings.WindowMode));
            SDL.SDL_SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");

            _context.CreateContext(_window);
            _context.Vsync = settings.Vsync;
            SDL.SDL_SetWindowGrab(_window, settings.MouseGrab ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);

            _performanceFrequency = SDL.SDL_GetPerformanceFrequency();
            _maxFps = settings.MaxFps;
            _maxTimestep = 1f / _maxFps;
        }

        public float GetTimestep()
        {
            var now = SDL.SDL_GetPerformanceCounter();
            if (_lastTime == 0) _lastTime = now;

            var step = (now - _lastTime) / _performanceFrequency;
            _lastTime = now;

            return step;
        }

        public void OnUpdate()
        {
            while (SDL.SDL_PollEvent(out var evnt) != 0) HandleEvent(evnt);

            _context.UpdateWindow();

            if (_maxFps == 0) return;

            {
                var now = SDL.SDL_GetPerformanceCounter();
                var step = (int)(1000 * (_maxTimestep - (now - _lastTime) / _performanceFrequency));

                if (step <= 0) return;

                Thread.Sleep(step);
            }

            while (true)
            {
                var now = SDL.SDL_GetPerformanceCounter();
                var step = (now - _lastTime) / _performanceFrequency;
                if (step >= _maxTimestep)
                    return;

                Thread.SpinWait(1000);
            }
        }

        public void Close()
        {
            _context.DeleteContext();

            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }

        private static SDL.SDL_WindowFlags GetWindowFlags(WindowMode windowMode)
        {
            var flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

            if ((windowMode & WindowMode.Fullscreen) != 0)
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            if ((windowMode & WindowMode.Borderless) != 0)
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;

            return flags;
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
