using System;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using SDL2;

namespace Pretend.Graphics.OpenGL
{
    public class OpenGLContext : IGraphicsContext
    {
        private IntPtr _context;
        private IntPtr _window;

        public void CreateContext(IntPtr windowPointer)
        {
            _window = windowPointer;
            _context = SDL.SDL_GL_CreateContext(windowPointer);

            GL.LoadBindings(new SDLContext());
        }

        public void UpdateWindow()
        {
            // TODO: Move these to the renderer
            GL.ClearColor(0.2f, 0.4f, 0.4f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SDL.SDL_GL_SwapWindow(_window);
        }

        public void DeleteContext()
        {
            SDL.SDL_GL_DeleteContext(_context);
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
