using System;
using Pretend;
using SDL2;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            var window = SDL.SDL_CreateWindow("Hello", 0, 0, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
            // Entrypoint.Start<SandboxApp>();
        }
    }
}
