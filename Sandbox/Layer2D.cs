using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Sandbox
{
    public class Layer2D : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _camera;
        private readonly IFactory _factory;
        private readonly IScene _scene;

        private ITexture2D _texture;
        private ITexture2D _texture2;
        private Vector3 _position;

        private float _leftSpeed;
        private float _rightSpeed;
        private float _upSpeed;
        private float _downSpeed;
        private PositionComponent _positionComponent;

        public Layer2D(I2DRenderer renderer, ICamera camera, IFactory factory, IScene scene)
        {
            _renderer = renderer;
            _camera = camera;
            _factory = factory;
            _scene = scene;
        }
        
        public void Attach()
        {
            _renderer.Init();

            _texture = _factory.Create<ITexture2D>();
            _texture.SetData("Assets/picture.png");

            _texture2 = _factory.Create<ITexture2D>();
            _texture2.SetData("Assets/picture2.png");

            _position = _camera.Position;

            _scene.Init();
            var entity = _scene.CreateEntity();
            entity.Components.AddRange(new IComponent[]
            {
                new PositionComponent { X = -100, Y = 400 },
                new SizeComponent { Width = 300, Height = 300 },
                new ColorComponent { Color = new Vector4(0.5f, 0.5f, 0.5f, 1f) }
            });

            entity = _scene.CreateEntity();
            _positionComponent = new PositionComponent {X = 400, Y = -100};
            entity.Components.AddRange(new IComponent[]
            {
                _positionComponent,
                new SizeComponent { Width = 400, Height = 300 },
                new ColorComponent { Color = new Vector4(1, 0, 1, 1) },
                new TextureComponent { Texture = _texture }
            });

            entity = _scene.CreateEntity();
            entity.Components.AddRange(new IComponent[]
            {
                new PositionComponent { X = -400, Y = -100 },
                new SizeComponent { Width = 300, Height = 300 },
                new TextureComponent { Texture = _texture2 }
            });
        }

        public void Update(float timeStep)
        {
            // Calculate location by speed
            var xSpeed = _rightSpeed - _leftSpeed;
            var ySpeed = _upSpeed - _downSpeed;

            _position.X += xSpeed * timeStep;
            _position.Y += ySpeed * timeStep;

            _positionComponent.Rotation += 100 * timeStep;
            if (_positionComponent.Rotation >= 360)
                _positionComponent.Rotation = 0;

            _camera.Position = _position;

            _renderer.Begin(_camera);

            _scene.Render();

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
