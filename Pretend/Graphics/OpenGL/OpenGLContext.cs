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

        private readonly float[] _vertices =
        {
             0.5f,  0.5f, 0.0f, // top right
             0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        public void CreateContext(IntPtr windowPointer)
        {
            _window = windowPointer;
            _context = SDL.SDL_GL_CreateContext(windowPointer);

            GL.LoadBindings(new SDLContext());

            // TODO Remove
            var vertexBuffer = new VertexBuffer(_vertices);
            var indexBuffer = new IndexBuffer(_indices);
            var vertexArray = new VertexArray();

            vertexArray.Bind();
            vertexBuffer.Bind();
            indexBuffer.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void UpdateWindow()
        {
            // TODO: Move these to the renderer
            GL.ClearColor(0.2f, 0.4f, 0.4f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

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
