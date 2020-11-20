using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SDL2;

namespace Pretend.Graphics.OpenGL
{
    public class OpenGLContext : IGraphicsContext
    {
        private IntPtr _context;
        private IntPtr _window;

        public void CreateContext(IntPtr windowPointer)
        {
            #if DEBUG
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 6);
            #endif
            _window = windowPointer;
            _context = SDL.SDL_GL_CreateContext(windowPointer);

            GL.LoadBindings(new SDLContext());
        }

        public void UpdateWindow()
        {
            SDL.SDL_GL_SwapWindow(_window);
        }

        public void DeleteContext()
        {
            SDL.SDL_GL_DeleteContext(_context);
        }

        public bool Vsync { set => SDL.SDL_GL_SetSwapInterval(value ? 1 : 0); }
    }

    public class SDLContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return SDL.SDL_GL_GetProcAddress(procName);
        }
    }
}
