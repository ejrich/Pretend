using System;
using Pretend;
using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Sandbox
{
    public class ExampleLayer : ILayer
    {
        private readonly IRenderer _renderer;
        private readonly ICamera _camera;

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

        private float x;
        private float y;

        public ExampleLayer(IRenderer renderer, ICamera camera)
        {
            _renderer = renderer;
            _camera = camera;
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
            _renderer.Begin(_camera);

            var transform = Matrix4.Identity;
            transform *= Matrix4.CreateTranslation(x, y, 0.0f);

            _shader.SetMat4("transform", transform);

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
                    y = 0.5f;
                    break;
                case KeyCode.S:
                    y = -0.5f;
                    break;
                case KeyCode.A:
                    x = -0.5f;
                    break;
                case KeyCode.D:
                    x = 0.5f;
                    break;
            }
        }

        private void HandleKeyRelease(KeyReleasedEvent evnt)
        {
            Console.WriteLine($"Released {evnt.KeyCode}");
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    y = 0f;
                    break;
                case KeyCode.S:
                    y = -0f;
                    break;
                case KeyCode.A:
                    x = -0f;
                    break;
                case KeyCode.D:
                    x = 0f;
                    break;
            }
        }
    }
}
