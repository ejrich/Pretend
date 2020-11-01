using OpenTK.Mathematics;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Sandbox
{
    public class TextLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;

        public TextLayer(ICamera camera, IScene scene)
        {
            _camera = camera;
            _scene = scene;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent {Camera = _camera, Active = true});

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new TextComponent
            {
                Text = "Hello world! yogurt pretty good",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 60
            });
        }

        public void Update(float timeStep)
        {
            _scene.Update(timeStep);
            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
        }
    }
}
