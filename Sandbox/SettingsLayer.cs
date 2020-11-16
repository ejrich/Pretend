using OpenTK.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Sandbox
{
    public class SettingsLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly ISettingsManager<Settings> _settingsManager;

        public SettingsLayer(ICamera camera, IScene scene, ISettingsManager<Settings> settingsManager)
        {
            _camera = camera;
            _scene = scene;
            _settingsManager = settingsManager;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var borderlessButton= _scene.CreateEntity();
            _scene.AddComponent(borderlessButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(borderlessButton, new TextComponent
            {
                Text = "Borderless",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });
        }

        public void Update(float timeStep)
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
