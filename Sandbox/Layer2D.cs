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
        private readonly ICamera _camera;
        private readonly IFactory _factory;
        private readonly IScene _scene;

        private ITexture2D _texture;
        private ITexture2D _texture2;

        public Layer2D(ICamera camera, IFactory factory, IScene scene)
        {
            _camera = camera;
            _factory = factory;
            _scene = scene;
        }

        public void Attach()
        {
            _texture = _factory.Create<ITexture2D>();
            _texture.SetData("Assets/picture.png");

            _texture2 = _factory.Create<ITexture2D>();
            _texture2.SetData("Assets/picture2.png");

            _scene.Init();

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new CameraComponent {Camera = _camera, Active = true});
            _scene.AddComponent(entity, new CameraScript(_camera));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = -100, Y = 400});
            _scene.AddComponent(entity, new SizeComponent {Width = 300, Height = 300});
            _scene.AddComponent(entity, new ColorComponent {Color = new Vector4(0.5f, 0.5f, 0.5f, 1f)});

            entity = _scene.CreateEntity();
            var positionComponent = new PositionComponent {X = 400, Y = -100};
            _scene.AddComponent(entity, positionComponent);
            _scene.AddComponent(entity, new SizeComponent {Width = 400, Height = 300});
            _scene.AddComponent(entity, new ColorComponent {Color = new Vector4(1, 0, 1, 1)});
            _scene.AddComponent(entity, new TextureComponent {Texture = _texture});
            _scene.AddComponent<IScriptComponent>(entity, new DiceScript(positionComponent));

            entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new PositionComponent {X = -400, Y = -100});
            _scene.AddComponent(entity, new SizeComponent {Width = 300, Height = 300});
            _scene.AddComponent(entity, new TextureComponent {Texture = _texture2});
        }

        public void Update(float timeStep)
        {
            _scene.Update(timeStep);
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            // Handle an event
            switch (evnt)
            {
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
                default:
                    _scene.HandleEvent(evnt);
                    break;
            }
        }
    }
}
