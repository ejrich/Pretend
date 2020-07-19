using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexArray : IVertexArray
    {
        private int _id;
        private IVertexBuffer _vertexBuffer;
        private IIndexBuffer _indexBuffer;

        public IVertexBuffer VertexBuffer
        {
            get => _vertexBuffer;
            set
            {
                Bind();
                value.Bind();

                // TODO Buffer layouts, this code will only support 3 element buffers
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                _vertexBuffer = value;
            }
        }
        public IIndexBuffer IndexBuffer
        {
            get => _indexBuffer;
            set 
            {
                Bind();
                value.Bind();

                _indexBuffer = value;
            }
        }

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
