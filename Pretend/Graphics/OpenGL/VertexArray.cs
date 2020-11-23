using System;
using OpenTK.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class VertexArray : IVertexArray
    {
        private readonly int _id;
        private IVertexBuffer _vertexBuffer;
        private IIndexBuffer _indexBuffer;
        private bool _disposed;

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

        public void Bind() => Bind(false);

        public void Bind(bool bindBuffers)
        {
            GL.BindVertexArray(_id);
            if (bindBuffers)
            {
                _vertexBuffer?.Bind();
                _indexBuffer?.Bind();
            }
        }

        public void Unbind() => GL.BindVertexArray(0);

        private static VertexAttribPointerType GetPointerType(Type type)
        {
            return type switch
            {
                { } floatType when floatType == typeof(float) => VertexAttribPointerType.Float,
                { } intType when intType == typeof(int) => VertexAttribPointerType.Int,
                { } boolType when boolType == typeof(bool) => VertexAttribPointerType.Int,
                _ => VertexAttribPointerType.Float
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            Unbind();
            GL.DeleteVertexArray(_id);
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();

            _disposed = true;
        }
    }
}
