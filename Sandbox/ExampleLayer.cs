using System;
using Pretend;
using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics;
using OpenToolkit.Mathematics;

namespace Sandbox
{
    public class ExampleLayer : ILayer
    {
        private readonly IRenderer _renderer;
        private readonly ICamera _camera;
        private readonly IFactory _factory;

        private readonly float[] _vertices =
        {
             400f,  300f, 0.0f, 1.0f, 1.0f, // top right
             400f, -300f, 0.0f, 1.0f, 0.0f, // bottom right
            -400f, -300f, 0.0f, 0.0f, 0.0f, // bottom left
            -400f,  300f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        private IShader _shader;
        private ITexture2D _texture;
        private IVertexArray _vertexArray;
        private Vector3 _position;

        public ExampleLayer(IRenderer renderer, ICamera camera, IFactory factory)
        {
            _renderer = renderer;
            _camera = camera;
            _factory = factory;
        }

        public void Attach()
        {
            _renderer.Init();

            var vertexBuffer = _factory.Create<IVertexBuffer>();
            vertexBuffer.SetData(_vertices);
            vertexBuffer.AddLayout<float>(3);
            vertexBuffer.AddLayout<float>(2);

            var indexBuffer = _factory.Create<IIndexBuffer>();
            indexBuffer.AddData(_indices);

            _vertexArray = _factory.Create<IVertexArray>();
            _vertexArray.VertexBuffer = vertexBuffer;
            _vertexArray.IndexBuffer = indexBuffer;

            _shader = _factory.Create<IShader>();
            _shader.Compile("Assets/shader.vert", "Assets/shader.frag");
            _shader.SetInt("texture0", 0);

            _texture = _factory.Create<ITexture2D>();
            _texture.SetData("Assets/picture.png");

            _position = _camera.Position;
        }

        public void Update(float timeStep)
        {
            // Do something
            _renderer.Begin(_camera);

            _texture.Bind();
            _renderer.Submit(_shader, _vertexArray);

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
            // Handle an event
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
                    break;
                case KeyReleasedEvent keyReleased:
                    HandleKeyRelease(keyReleased);
                    break;
                // case WindowResizeEvent resize:
                //     _camera.Resize(resize.Width, resize.Height);
                //     break;
            }
        }

        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            Console.WriteLine($"Pressing {evnt.KeyCode}");
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _position.Y = 200f;
                    break;
                case KeyCode.S:
                    _position.Y = -200f;
                    break;
                case KeyCode.A:
                    _position.X = -200f;
                    break;
                case KeyCode.D:
                    _position.X = 200f;
                    break;
            }
            _camera.Position = _position;
        }

        private void HandleKeyRelease(KeyReleasedEvent evnt)
        {
            Console.WriteLine($"Released {evnt.KeyCode}");
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _position.Y = 0f;
                    break;
                case KeyCode.S:
                    _position.Y = 0f;
                    break;
                case KeyCode.A:
                    _position.X = 0f;
                    break;
                case KeyCode.D:
                    _position.X = 0f;
                    break;
            }
            _camera.Position = _position;
        }
    }
}
