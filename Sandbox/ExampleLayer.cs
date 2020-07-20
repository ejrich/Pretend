using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;

namespace Sandbox
{
    public class ExampleLayer : ILayer
    {
        private readonly IRenderer _renderer;

        private readonly float[] _vertices =
        {
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        private IShader _shader;
        private ITexture2D _texture;
        private IVertexArray _vertexArray;

        public ExampleLayer(IRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Attach()
        {
            _renderer.Init();

            var vertexBuffer = new VertexBuffer();
            vertexBuffer.SetData(_vertices);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);

            var indexBuffer = new IndexBuffer();
            indexBuffer.AddData(_indices);

            _vertexArray = new VertexArray
            {
                VertexBuffer = vertexBuffer,
                IndexBuffer = indexBuffer
            };

            _shader = new Shader();
            _shader.Compile("Assets/shader.vert", "Assets/shader.frag");
            _shader.SetInt("texture0", 0);

            _texture = new Texture2D();
            _texture.SetData("Assets/picture.png");
        }

        public void Update(float timeStep)
        {
            // Do something
            _renderer.Begin();

            _texture.Bind();
            _renderer.Submit(_shader, _vertexArray);

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
            // Handle an event
        }
    }
}
