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
            var vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(_vertices);

            var indexBuffer = new IndexBuffer();
            indexBuffer.AddData(_indices);

            var vertexArray = new VertexArray
            {
                VertexBuffer = vertexBuffer,
                IndexBuffer = indexBuffer
            };

            var shader = new Shader();
            shader.Compile("Assets/shader.vert", "Assets/shader.frag");
            shader.Bind();
        }

        public void Update(float timeStep)
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
