using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Physics;

namespace Game
{
    public class GameLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly IPhysicsContainer _physicsContainer;
        private readonly IGame _game;

        public GameLayer(ICamera camera, IScene scene, IPhysicsContainer physicsContainer, IGame game)
        {
            _camera = camera;
            _scene = scene;
            _physicsContainer = physicsContainer;
            _game = game;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var playerEntity = _scene.CreateEntity();
            var playerPosition = new PositionComponent { Y = 450 };
            var physicsComponent = new PhysicsComponent();
            _scene.AddComponent(playerEntity, playerPosition);
            _scene.AddComponent(playerEntity, new SizeComponent { Width = 30, Height = 30 });
            _scene.AddComponent(playerEntity, new ColorComponent { Color = new Vector4(1, 0, 0, 1) });
            _scene.AddComponent(playerEntity, physicsComponent);
            _scene.AddComponent(playerEntity, new PlayerScript(playerPosition, physicsComponent));

            var floorEntity = _scene.CreateEntity();
            _scene.AddComponent(floorEntity, new PositionComponent { Y = -25 });
            _scene.AddComponent(floorEntity, new SizeComponent { Width = 1280, Height = 20 });
            _scene.AddComponent(floorEntity, new PhysicsComponent { Fixed = true });

            _game.Init(_scene, _physicsContainer, playerPosition);
        }

        public void Update(float timeStep)
        {
            // Update the game
            _game.Update(timeStep);

            _scene.Update(timeStep);
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            _scene.HandleEvent(evnt);
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
                case KeyCode.Enter:
                    if (!_game.Running) _game.Reset();
                    break;
            }
        }
    }
}
