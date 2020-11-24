using System;
using OpenTK.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class IndexBuffer : IIndexBuffer
    {
        private readonly int _id;
        private bool _disposed;

        public IndexBuffer() => _id = GL.GenBuffer();

        public int Count { get; private set; }

        public void AddData(uint[] indices)
        {
            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            Count += indices.Length;
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);

        public void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            Unbind();
            GL.DeleteBuffer(_id);

            _disposed = true;
        }
    }
}
