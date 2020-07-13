using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexBuffer : IVertexBuffer
    {
        private int _id;

        public VertexBuffer(float[] vertices)
        {
            _id = GL.GenBuffer();
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        ~VertexBuffer()
        {
            GL.DeleteBuffer(_id);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetData() {}
    }
}
