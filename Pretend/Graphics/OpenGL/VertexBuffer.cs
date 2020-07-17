using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexBuffer : IVertexBuffer
    {
        private int _id;

        public VertexBuffer()
        {
            _id = GL.GenBuffer();
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
