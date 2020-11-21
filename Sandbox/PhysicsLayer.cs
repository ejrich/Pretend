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
        private readonly ISoundManager _soundManager;
        private readonly ISandbox _sandbox;

        public PhysicsLayer(ICamera camera, IScene scene, IPhysicsContainer physicsContainer, IFactory factory,
            ISoundManager soundManager, ISandbox sandbox)
        {
            _camera = camera;
            _scene = scene;
            _physicsContainer = physicsContainer;
            _factory = factory;
            _soundManager = soundManager;
            _sandbox = sandbox;
        }

        public void Attach()
        {
            _scene.Init();
            _physicsContainer.Gravity = new Vector3(0, -800, 0);

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new CameraComponent {Camera = _camera, Active = true});

            var texture = _factory.Create<ITexture2D>();
            texture.SetData("Assets/landscape.jpeg");

            var soundBuffer = _soundManager.CreateSoundBuffer();
            soundBuffer.SetData("Assets/jump.wav");

            var source = _soundManager.CreateSource();
            source.Gain = 1f;

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = -400, Y = -100});
            _scene.AddComponent(entity, new SizeComponent {Width = 100, Height = 100});
            _scene.AddComponent(entity, new TextureComponent { Texture = texture});
            var physicsComponent = new PhysicsComponent();
            _scene.AddComponent(entity, physicsComponent);
            var sourceComponent = new SourceComponent { Source = source, SoundBuffer = soundBuffer };
            _scene.AddComponent(entity, sourceComponent);
            _scene.AddComponent(entity, new ControlScript(physicsComponent, sourceComponent));

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
            _soundManager.Start(60, _scene.EntityContainer);
        }

        public void Detach()
        {
            _physicsContainer.Stop();
            _soundManager.Stop();
            _soundManager.Dispose();
        }

        public void Pause()
        {
            _physicsContainer.Stop();
            _soundManager.Stop();
            Paused = true;
        }

        public void Resume()
        {
            _physicsContainer.Start(144, _scene.EntityContainer);
            _soundManager.Start(60, _scene.EntityContainer);
            Paused = false;
        }

        public bool Paused { get; private set; }

        public void Update(float timeStep)
        {
            // if (_sandbox.ActiveLayer != ActiveLayer.PhysicsLayer) return;

            _scene.Update(timeStep);
        }

        public void Render()
        {
            // if (_sandbox.ActiveLayer != ActiveLayer.PhysicsLayer) return;
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
                        if (Paused)
                            Resume();
                        else
                            Pause();
                    }
                    break;
            }
        }
    }
}
