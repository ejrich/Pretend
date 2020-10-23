using OpenTK.Mathematics;
using Pretend;
using Pretend.Audio;
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
        private readonly IScene _scene;
        private readonly IPhysicsContainer _physicsContainer;
        private readonly IFactory _factory;

        public PhysicsLayer(ICamera camera, IScene scene, IPhysicsContainer physicsContainer, IFactory factory)
        {
            _camera = camera;
            _scene = scene;
            _physicsContainer = physicsContainer;
            _factory = factory;
        }

        public void Attach()
        {
            _scene.Init();
            _physicsContainer.Gravity = new Vector3(0, -800, 0);

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new CameraComponent {Camera = _camera, Active = true});

            var texture = _factory.Create<ITexture2D>();
            texture.SetData("Assets/landscape.jpeg");

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = -400, Y = -100});
            _scene.AddComponent(entity, new SizeComponent {Width = 100, Height = 100});
            _scene.AddComponent(entity, new TextureComponent { Texture = texture});
            var physicsComponent = new PhysicsComponent();
            _scene.AddComponent(entity, physicsComponent);
            _scene.AddComponent(entity, new ControlScript(physicsComponent));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Y = -360});
            _scene.AddComponent(entity, new SizeComponent {Width = 1280, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Y = -200});
            _scene.AddComponent(entity, new SizeComponent {Width = 600, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = 420, Y = -95, Yaw = 45});
            _scene.AddComponent(entity, new SizeComponent {Width = 300, Height = 10});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {Y = -100});
            _scene.AddComponent(entity, new SizeComponent {Width = 10, Height = 300});
            _scene.AddComponent(entity, new PhysicsComponent {Fixed = true });

            _physicsContainer.Start(144, _scene.EntityContainer);
            var sound = new Sound();
            sound.Play("Assets/sound.wav");
        }

        public void Update(float timeStep)
        {
            _scene.Update(timeStep);
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
                case KeyPressedEvent keyPressed:
                    if (keyPressed.KeyCode == KeyCode.P)
                    {
                        if (_physicsContainer.Running)
                            _physicsContainer.Stop();
                        else
                            _physicsContainer.Start(144, _scene.EntityContainer);
                    }
                    break;
            }
        }
    }
}
