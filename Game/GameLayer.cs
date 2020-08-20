using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Game
{
    public class GameLayer : ILayer
    {
        private readonly I2DRenderer _renderer;
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly IGame _game;

        private Renderable2DObject _playerObject;
        private Renderable2DObject _floor;

        public GameLayer(I2DRenderer renderer, ICamera camera, IScene scene, IGame game)
        {
            _renderer = renderer;
            _camera = camera;
            _scene = scene;
            _game = game;
        }

        public void Attach()
        {
            // _renderer.Init();
            //
            // _playerObject = new Renderable2DObject
            // {
            //     Width = 30, Height = 30,
            //     Color = new Vector4(1, 0, 0, 1)
            // };
            // _floor = new Renderable2DObject
            // {
            //     Y = -25,
            //     Width = 1280, Height = 20
            // };
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var playerEntity = _scene.CreateEntity();
            var playerPosition = new PositionComponent { Y = 450 };
            _scene.AddComponent(playerEntity, playerPosition);
            _scene.AddComponent(playerEntity, new SizeComponent { Width = 30, Height = 30 });
            _scene.AddComponent(playerEntity, new ColorComponent { Color = new Vector4(1, 0, 0, 1) });
            _scene.AddComponent<IScriptComponent>(playerEntity, new PlayerScript(playerPosition));

            var floorEntity = _scene.CreateEntity();
            _scene.AddComponent(floorEntity, new PositionComponent { Y = -25 });
            _scene.AddComponent(floorEntity, new SizeComponent { Width = 1280, Height = 20 });
        }

        public void Update(float timeStep)
        {
            // Update the game
            _game.Update(timeStep);
            // _playerObject.Y = _game.PlayerPosition;
            // _playerObject.Rotation = _game.PlayerRotation;

            // _renderer.Begin(_camera);

            // _renderer.Submit(_playerObject);
            // _renderer.Submit(_floor);
            // foreach (var obstaclePosition in _game.ObstaclePositions)
            // {
            //     _renderer.Submit(new Renderable2DObject
            //     {
            //         X = obstaclePosition.X,
            //         Width = 40, Height = 40
            //     });
            // }
            //
            // _renderer.End();

            _scene.Update(timeStep);
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                // case KeyPressedEvent keyPressed:
                //     HandleKeyPress(keyPressed);
                //     break;
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
                default:
                    _scene.HandleEvent(evnt);
                    break;
            }
        }

        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                // case KeyCode.Space:
                //     _game.Jump();
                //     break;
                case KeyCode.Enter:
                    if (!_game.Running) _game.Reset();
                    break;
            }
        }
    }
}
