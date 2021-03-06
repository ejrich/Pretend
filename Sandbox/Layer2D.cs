using System.Numerics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Physics;

namespace Sandbox
{
    public class Layer2D : ILayer
    {
        private readonly ICamera _camera;
        private readonly IFactory _factory;
        private readonly IScene _scene;
        private readonly IPhysicsContainer _physicsContainer;

        private ITexture2D _texture;
        private ITexture2D _texture2;

        public Layer2D(ICamera camera, IFactory factory, IScene scene, IPhysicsContainer physicsContainer)
        {
            _camera = camera;
            _factory = factory;
            _scene = scene;
            _physicsContainer = physicsContainer;
        }

        public void Attach()
        {
            _texture = _factory.Create<ITexture2D>();
            _texture.SetData("Assets/picture.png");

            _texture2 = _factory.Create<ITexture2D>();
            _texture2.SetData("Assets/picture2.png");

            _scene.Init();
            _physicsContainer.Gravity = new Vector3(0, -800, 0);

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new CameraComponent {Camera = _camera, Active = true});
            _scene.AddComponent(entity, new CameraScript(_camera));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Position = new Vector3(-100, 400, 0)});
            _scene.AddComponent(entity, new SizeComponent {Width = 300, Height = 300});
            _scene.AddComponent(entity, new ColorComponent {Color = new Vector4(0.5f, 0.5f, 0.5f, 1f)});

            entity = _scene.CreateEntity();
            var positionComponent = new PositionComponent {Position = new Vector3(400, -100, 0)};
            _scene.AddComponent(entity, positionComponent);
            _scene.AddComponent(entity, new SizeComponent {Width = 400, Height = 300});
            _scene.AddComponent(entity, new ColorComponent {Color = new Vector4(1, 0, 1, 1)});
            _scene.AddComponent(entity, new TextureComponent {Texture = _texture});
            _scene.AddComponent(entity, new DiceScript(positionComponent));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Position = new Vector3(-400, -100, 0) });
            _scene.AddComponent(entity, new SizeComponent {Width = 300, Height = 300});
            _scene.AddComponent(entity, new TextureComponent {Texture = _texture2});
            _scene.AddComponent(entity, new PhysicsComponent {Velocity = new Vector3(300, 500, 0)});

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Position = new Vector3(0, -360, 0)});
            _scene.AddComponent(entity, new SizeComponent {Width = 1280, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true});
        }

        public void Detach()
        {
            _texture.Dispose();
            _texture2.Dispose();
        }

        public bool Paused { get; }

        public void Update(float timeStep)
        {
            _physicsContainer.Simulate(timeStep, _scene.EntityContainer);
            _scene.Update(timeStep);
        }

        public void Render()
        {
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            _scene.HandleEvent(evnt);
            // Handle an event
            switch (evnt)
            {
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }
    }
}
