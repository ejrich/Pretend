using System.Numerics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Text;
using Pretend.UI;

namespace Sandbox
{
    public class TextLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly IFactory _factory;

        public TextLayer(ICamera camera, IScene scene, IFactory factory)
        {
            _camera = camera;
            _scene = scene;
            _factory = factory;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new TextComponent
            {
                Text = "Hello world! yogurt pretty good\nHello again!",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 15,
                // Alignment = TextAlignment.Left,
                // Orientation = new Vector3(0, 0, 45)
            });

            var input = _factory.Create<Pretend.UI.IInput>();
            input.Init(_scene, new InputSettings
            {
                Position = new Vector3(0, 100, 0), Size = new Vector2(200, 40),
                Font = "Assets/Roboto-Medium.ttf", FontSize = 30, FontColor = new Vector4(0, 0, 0 ,1),
            });
        }

        public bool Paused { get; }

        public void Update(float timeStep)
        {
            _scene.Update(timeStep);
        }

        public void Render()
        {
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            _scene.HandleEvent(evnt);

            switch (evnt)
            {
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }
    }
}
