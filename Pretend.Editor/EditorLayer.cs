using System;
using OpenToolkit.Mathematics;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Pretend.Editor
{
    public class EditorLayer : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _viewportCamera;
        private readonly ICamera _mainCamera;
        private readonly IFactory _factory;
        private readonly IRenderContext _renderContext;

        private Vector3 _position;
        private IFramebuffer _framebuffer;
        private int _width;
        private int _height;

        private float _leftSpeed;
        private float _rightSpeed;
        private float _upSpeed;
        private float _downSpeed;
        private float _rotation;

        public EditorLayer(I2DRenderer renderer, ICamera viewportCamera, ICamera mainCamera, IFactory factory,
            IRenderContext renderContext, IWindowAttributesProvider windowAttributes)
        {
            _renderer = renderer;
            _viewportCamera = viewportCamera;
            _mainCamera = mainCamera;
            _factory = factory;
            _renderContext = renderContext;
            _width = windowAttributes.Width;
            _height = windowAttributes.Height;
        }
        
        public void Attach()
        {
            _renderer.Init();
            
            _framebuffer = _factory.Create<IFramebuffer>();
            _framebuffer.Init();

            _viewportCamera.Resize(_width * 3 / 4, _height);
            _position = _viewportCamera.Position;
        }

        public void Update(float timeStep)
        {
            // Thread.Sleep(16);
            // Calculate location by speed
            var xSpeed = _rightSpeed - _leftSpeed;
            var ySpeed = _upSpeed - _downSpeed;

            _position.X += xSpeed * timeStep;
            _position.Y += ySpeed * timeStep;

            _rotation += 100 * timeStep;
            if (_rotation >= 360) _rotation = 0;

            _viewportCamera.Position = _position;

            // Capture the framebuffer for the viewport
            _framebuffer.Bind();
            _renderContext.BackgroundColor(0, 1, 1, 1);
            _renderer.Begin(_viewportCamera);

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
                Width = 300, Height = 300
            });

            _renderer.End();
            _framebuffer.Unbind();

            // Render the viewport and the rest of the editor panels
            _renderContext.BackgroundColor(0.2f, 0.2f, 0.2f, 1);
            _renderer.Begin(_mainCamera);
            _renderer.Submit(new Renderable2DObject
            {
                X = _width / 8f, Width = Convert.ToUInt32(_width * 3 / 4), Height = Convert.ToUInt32(_height),
                Texture = _framebuffer.ColorTexture
            });
            _renderer.Submit(new Renderable2DObject
            {
                X = -480, Width = 300, Height = 700,
                Color = new Vector4(0.1f, 0.1f, 0.1f, 1)
            });
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
                case WindowResizeEvent resize:
                    _width = resize.Width;
                    _height = resize.Height;
                    _mainCamera.Resize(resize.Width, resize.Height);
                    _viewportCamera.Resize(resize.Width * 3 / 4, resize.Height);
                    _framebuffer.Resize(resize.Width, resize.Height);
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
