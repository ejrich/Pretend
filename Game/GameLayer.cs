using OpenToolkit.Mathematics;
using Pretend;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Game
{
    public class GameLayer : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _camera;
        private readonly IGame _game;

        private Renderable2DObject _playerObject;
        private Renderable2DObject _floor;
        private bool _jumping;
        private float _jumpTime;

        public GameLayer(I2DRenderer renderer, ICamera camera, IGame game)
        {
            _renderer = renderer;
            _camera = camera;
            _game = game;
        }

        public void Attach()
        {
            _renderer.Init();

            _playerObject = new Renderable2DObject
            {
                Width = 30, Height = 30,
                Color = new Vector4(1, 0, 0, 1)
            };
            _floor = new Renderable2DObject
            {
                Y = -25,
                Width = 1280, Height = 20
            };
        }

        public void Update(float timeStep)
        {
            // Move the object
            if (_jumping)
            {
                _jumpTime += timeStep;
                var position = 300 * _jumpTime + -400 * _jumpTime * _jumpTime;

                _playerObject.Y = position < 0 ? 0 : position;
                if (position <= 0)
                {
                    _jumpTime = 0;
                    _jumping = false;
                }
            }

            _game.Update(timeStep);

            _renderer.Begin(_camera);

            _renderer.Submit(_playerObject);
            _renderer.Submit(_floor);
            foreach (var obstaclePosition in _game.ObstaclePositions)
            {
                _renderer.Submit(new Renderable2DObject
                {
                    X = obstaclePosition,
                    Width = 40, Height = 40
                });
            }

            _renderer.End();
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
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
                case KeyCode.Space:
                    _jumping = true;
                    break;
            }
        }
    }
}
