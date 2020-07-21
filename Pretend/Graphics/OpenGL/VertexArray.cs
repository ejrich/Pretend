using System;
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

                var index = 0;
                foreach (var layout in value.Layouts)
                {
                    GL.EnableVertexAttribArray(index);
                    GL.VertexAttribPointer(index, layout.Count, GetPointerType(layout.Type),
                        layout.Normalized, value.Stride, layout.Offset);
                    index++;
                }

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

        private VertexAttribPointerType GetPointerType(Type type)
        {
            switch (type)
            {
                case Type floatType when floatType == typeof(float):
                    return VertexAttribPointerType.Float;

                case Type intType when intType == typeof(int):
                    return VertexAttribPointerType.Int;

                case Type boolType when boolType == typeof(bool):
                    return VertexAttribPointerType.Int;

                default: return VertexAttribPointerType.Float;
            }
        }
    }
}
