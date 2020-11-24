using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexBuffer : IVertexBuffer
    {
        private readonly int _id;
        private readonly List<BufferLayout> _layouts = new List<BufferLayout>();
        private bool _disposed;

        public IEnumerable<BufferLayout> Layouts => _layouts;
        public int Stride { get; private set; }

        public VertexBuffer() => _id = GL.GenBuffer();

        public void SetSize<T>(int count) where T : struct
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf<T>() * count, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void SetData(float[] vertices)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        public void AddData<T>(T[] data) where T : struct
        {
            Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, Marshal.SizeOf<T>() * data.Length, data);
        }

        public void AddLayout<T>(int count, bool normalized = false) where T : struct
        {
            _layouts.Add(new BufferLayout
            {
                Type = typeof(T),
                Count = count,
                Offset = Stride,
                Normalized = normalized
            });

            Stride += Marshal.SizeOf<T>() * count;
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, _id);

        public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

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
