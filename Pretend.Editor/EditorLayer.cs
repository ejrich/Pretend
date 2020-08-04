using OpenToolkit.Mathematics;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Pretend.Editor
{
    public class EditorLayer : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _camera;
        private readonly IFactory _factory;

        private Vector3 _position;
        private IFramebuffer _framebuffer;

        private float _leftSpeed;
        private float _rightSpeed;
        private float _upSpeed;
        private float _downSpeed;
        private float _rotation;

        public EditorLayer(I2DRenderer renderer, ICamera camera, IFactory factory)
        {
            _renderer = renderer;
            _camera = camera;
            _factory = factory;
        }
        
        public void Attach()
        {
            _renderer.Init();

            _framebuffer = _factory.Create<IFramebuffer>();
            _framebuffer.Init();

            _position = _camera.Position;
        }

        public void Update(float timeStep)
        {
            // Calculate location by speed
            var xSpeed = _rightSpeed - _leftSpeed;
            var ySpeed = _upSpeed - _downSpeed;

            _position.X += xSpeed * timeStep;
            _position.Y += ySpeed * timeStep;

            _rotation += 100 * timeStep;
            if (_rotation >= 360) _rotation = 0;

            _camera.Position = _position;

            // _framebuffer.Bind();
            _renderer.Begin(_camera);

            _renderer.Submit(new Renderable2DObject
            {
                X = -100, Y = 400,
                Width = 300, Height = 300,
                Color = new Vector4(0.5f, 0.5f, 0.5f, 1f),
            });
            _renderer.Submit(new Renderable2DObject
            {
                X = 400, Y = -100,
                Width = 400, Height = 300, Rotation = _rotation,
                Color = new Vector4(1, 0, 1, 1),
            });
            _renderer.Submit(new Renderable2DObject
            {
                X = -400, Y = -100,
                Width = 300, Height = 300,
            });

            _renderer.End();
            // _framebuffer.Unbind();
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
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }

        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _upSpeed = 1;
                    break;
                case KeyCode.S:
                    _downSpeed = 1;
                    break;
                case KeyCode.A:
                    _leftSpeed = 1;
                    break;
                case KeyCode.D:
                    _rightSpeed = 1;
                    break;
            }
        }

        private void HandleKeyRelease(KeyReleasedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _upSpeed = 0;
                    break;
                case KeyCode.S:
                    _downSpeed = 0;
                    break;
                case KeyCode.A:
                    _leftSpeed = 0;
                    break;
                case KeyCode.D:
                    _rightSpeed = 0;
                    break;
            }
        }
    }
}
