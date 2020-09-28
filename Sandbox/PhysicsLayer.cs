using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Physics;

namespace Sandbox
{
    public class PhysicsLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IFactory _factory;
        private readonly IScene _scene;
        private readonly IPhysicsContainer _physicsContainer;

        public PhysicsLayer(ICamera camera, IFactory factory, IScene scene, IPhysicsContainer physicsContainer)
        {
            _camera = camera;
            _factory = factory;
            _scene = scene;
            _physicsContainer = physicsContainer;
        }

        public void Attach()
        {
            _scene.Init();
            _physicsContainer.Gravity = new Vector3(0, -800, 0);

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new CameraComponent {Camera = _camera, Active = true});

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = -400, Y = -100});
            _scene.AddComponent(entity, new SizeComponent {Width = 100, Height = 100});
            var physicsComponent = new PhysicsComponent();
            _scene.AddComponent(entity, physicsComponent);
            _scene.AddComponent(entity, new ControlScript(physicsComponent));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Y = -360});
            _scene.AddComponent(entity, new SizeComponent {Width = 1280, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Y = -360, Rotation = 30});
            _scene.AddComponent(entity, new SizeComponent {Width = 1280, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });
        }

        public void Update(float timeStep)
        {
            _scene.Update(timeStep);
            _physicsContainer.Simulate(timeStep, _scene.EntityContainer);
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
