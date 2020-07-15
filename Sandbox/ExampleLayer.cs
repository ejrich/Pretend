using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics.OpenGL;
using OpenToolkit.Graphics.OpenGL4;

namespace Sandbox
{
    public class ExampleLayer : ILayer
    {
        private readonly float[] _vertices =
        {
             0.5f,  0.5f, 0.0f, // top right
             0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        public void Attach()
        {
            var vertexBuffer = new VertexBuffer(_vertices);
            var indexBuffer = new IndexBuffer(_indices);
            var vertexArray = new VertexArray();

            vertexArray.Bind();
            vertexBuffer.Bind();
            indexBuffer.Bind();

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Update()
        {
            // Do something
            GL.ClearColor(0.2f, 0.4f, 0.4f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void HandleEvent(IEvent evnt)
        {
            // Handle an event
        }
    }
}
