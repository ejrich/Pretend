using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexBuffer : IVertexBuffer
    {
        private int _id;
        private List<BufferLayout> _layouts;

        public IEnumerable<BufferLayout> Layouts => _layouts;
        public int Stride { get; private set; }

        public VertexBuffer()
        {
            _id = GL.GenBuffer();
            _layouts = new List<BufferLayout>();
        }

        ~VertexBuffer()
        {
            GL.DeleteBuffer(_id);
        }

        public void SetData(float[] vertices)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
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

            Stride += Marshal.SizeOf(default(T)) * count;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
