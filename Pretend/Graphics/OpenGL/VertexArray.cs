using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexArray : IVertexArray
    {
        private int _id;

        public IVertexBuffer VertexBuffer { get; set;}
        public IIndexBuffer IndexBuffer { get; set; }

        public VertexArray()
        {
            _id = GL.GenVertexArray();
        }

        ~VertexArray()
        {
            GL.DeleteVertexArray(_id);
        }

        public void Bind()
        {
            GL.BindVertexArray(_id);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}