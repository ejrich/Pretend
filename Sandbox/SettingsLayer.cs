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

        private bool _visible;

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

            var fullscreenButton= _scene.CreateEntity();
            _scene.AddComponent(fullscreenButton, new PositionComponent { X = 530, Y = 150 });
            _scene.AddComponent(fullscreenButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(fullscreenButton, new SettingsScript());
            _scene.AddComponent(fullscreenButton, new TextComponent
            {
                Text = "Fullscreen",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });

            var borderlessButton= _scene.CreateEntity();
            _scene.AddComponent(borderlessButton, new PositionComponent { X = 530, Y = 100 });
            _scene.AddComponent(borderlessButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(borderlessButton, new TextComponent
            {
                Text = "Borderless",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });

            var vsyncButton= _scene.CreateEntity();
            _scene.AddComponent(vsyncButton, new PositionComponent { X = 530, Y = 50 });
            _scene.AddComponent(vsyncButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(vsyncButton, new TextComponent
            {
                Text = "Vsync",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });

            var fpsButton= _scene.CreateEntity();
            _scene.AddComponent(fpsButton, new PositionComponent { X = 530 });
            _scene.AddComponent(fpsButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(fpsButton, new TextComponent
            {
                Text = "FPS",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });

            var resolutionButton= _scene.CreateEntity();
            _scene.AddComponent(resolutionButton, new PositionComponent { X = 530, Y = -50 });
            _scene.AddComponent(resolutionButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(resolutionButton, new TextComponent
            {
                Text = "Resolution",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });

            var applyButton= _scene.CreateEntity();
            _scene.AddComponent(applyButton, new PositionComponent { X = 530, Y = -100 });
            _scene.AddComponent(applyButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(applyButton, new TextComponent
            {
                Text = "Apply",
                Font = "Assets/Roboto-Medium.ttf",
                Size = 30,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            });
        }

        public void Update(float timeStep)
        {
            if (!_visible) return;

            _scene.Render();
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
                case KeyPressedEvent keyPressed:
                    if (keyPressed.KeyCode == KeyCode.Escape)
                        _visible = !_visible;
                    break;
            }

            if (!_visible) return;

            _scene.HandleEvent(evnt);
        }
    }
}
