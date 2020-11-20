using OpenTK.Mathematics;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Text;

namespace Sandbox
{
    public class TextLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly ISandbox _sandbox;

        public TextLayer(ICamera camera, IScene scene, ISandbox sandbox)
        {
            _camera = camera;
            _scene = scene;
            _sandbox = sandbox;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent {Camera = _camera, Active = true});

            var entity = _scene.CreateEntity();
            _scene.AddComponent(entity, new TextComponent
            {
                Text = "Hello world! yogurt pretty good\nHello again!",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 15,
                // Alignment = TextAlignment.Left,
                // Orientation = new Vector3(0, 0, 45)
            });
        }

        public bool Paused { get; }

        public void Update(float timeStep)
        {
            if (_sandbox.ActiveLayer != ActiveLayer.TextLayer) return;

            _scene.Update(timeStep);
        }

        public void Render()
        {
            if (_sandbox.ActiveLayer != ActiveLayer.TextLayer) return;

            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }
    }
}
