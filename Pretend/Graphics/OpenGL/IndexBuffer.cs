using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class IndexBuffer : IIndexBuffer
    {
        private readonly int _id;

        public IndexBuffer()
        {
            _id = GL.GenBuffer();
        }

        ~IndexBuffer()
        {
            GL.DeleteBuffer(_id);
        }

        public int Count { get; private set; }

        public void AddData(uint[] indices)
        {
            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            Count += indices.Length;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
