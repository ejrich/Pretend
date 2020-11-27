using System.Numerics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.UI;

namespace Sandbox
{
    public class SettingsLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly ISettingsManager<Settings> _settingsManager;
        private readonly ILayerContainer _layerContainer;

        private static readonly Vector4 SelectedColor = new Vector4(0, 1, 1, 1);

        private Settings _settings => _settingsManager.Settings;
        private bool _visible;

        public SettingsLayer(ICamera camera, IScene scene, ISettingsManager<Settings> settingsManager,
            ILayerContainer layerContainer)
        {
            _camera = camera;
            _scene = scene;
            _settingsManager = settingsManager;
            _layerContainer = layerContainer;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var fullscreenButton = _scene.CreateEntity();
            var button = new Button();
            button.Init(_scene, fullscreenButton, new ButtonSettings
            {
                Text = "Fullscreen", Font = "Assets/Roboto-Medium.ttf", FontSize = 30,
                FontColor = new Vector4(0, 0, 0, 1), Position = new Vector3(530, 150, 0),
                Size = new Vector2(200, 40)
            });
            button.OnRelease = _ =>
            {
                if (_settings.WindowMode.HasFlag(WindowMode.Fullscreen))
                    _settings.WindowMode &= ~WindowMode.Fullscreen;
                else
                    _settings.WindowMode |= WindowMode.Fullscreen;
            };
            button.Update = _ =>
            {
                if (_settings.WindowMode.HasFlag(WindowMode.Fullscreen))
                {
                    if (_._color.Color == Vector4.One)
                        _._color.Color = SelectedColor;
                }
                else if (_._color.Color == SelectedColor)
                    _._color.Color = Vector4.One;
            };

            var borderlessButton= _scene.CreateEntity();
            _scene.AddComponent(borderlessButton, new PositionComponent { Position = new Vector3(530, 100, 0) });
            _scene.AddComponent(borderlessButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(borderlessButton, new ColorComponent());
            _scene.AddComponent(borderlessButton, new SettingsScript(_settingsManager,
                _ =>
                {
                    if (_.Settings.WindowMode.HasFlag(WindowMode.Borderless))
                        _.Settings.WindowMode &= ~WindowMode.Borderless;
                    else
                        _.Settings.WindowMode |= WindowMode.Borderless;
                }, _ => _.Settings.WindowMode.HasFlag(WindowMode.Borderless), borderlessButton));
            _scene.AddComponent(borderlessButton, CreateButtonText("Borderless"));

            var vsyncButton= _scene.CreateEntity();
            _scene.AddComponent(vsyncButton, new PositionComponent { Position = new Vector3(530, 50, 0) });
            _scene.AddComponent(vsyncButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(vsyncButton, new ColorComponent());
            _scene.AddComponent(vsyncButton, new SettingsScript(_settingsManager,
                _ => _.Settings.Vsync = !_.Settings.Vsync, _ => _.Settings.Vsync, vsyncButton));
            _scene.AddComponent(vsyncButton, CreateButtonText("Vsync"));

            var zeroFpsButton= _scene.CreateEntity();
            _scene.AddComponent(zeroFpsButton, new PositionComponent { Position = new Vector3(460, 0, 0) });
            _scene.AddComponent(zeroFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(zeroFpsButton, new ColorComponent());
            _scene.AddComponent(zeroFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 0, _ => _.Settings.MaxFps == 0, zeroFpsButton));
            _scene.AddComponent(zeroFpsButton, CreateButtonText("0"));

            var sixtyFpsButton= _scene.CreateEntity();
            _scene.AddComponent(sixtyFpsButton, new PositionComponent { Position = new Vector3(530, 0, 0) });
            _scene.AddComponent(sixtyFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(sixtyFpsButton, new ColorComponent());
            _scene.AddComponent(sixtyFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 60, _ => _.Settings.MaxFps == 60, sixtyFpsButton));
            _scene.AddComponent(sixtyFpsButton, CreateButtonText("60"));

            var highFpsButton= _scene.CreateEntity();
            _scene.AddComponent(highFpsButton, new PositionComponent { Position = new Vector3(600, 0, 0) });
            _scene.AddComponent(highFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(highFpsButton, new ColorComponent());
            _scene.AddComponent(highFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 144, _ => _.Settings.MaxFps == 144, highFpsButton));
            _scene.AddComponent(highFpsButton, CreateButtonText("144"));

            var resolution1Button= _scene.CreateEntity();
            _scene.AddComponent(resolution1Button, new PositionComponent { Position = new Vector3(479, -50, 0) });
            _scene.AddComponent(resolution1Button, new SizeComponent { Width = 98, Height = 40 });
            _scene.AddComponent(resolution1Button, new ColorComponent());
            _scene.AddComponent(resolution1Button, new SettingsScript(_settingsManager,
                _ =>
                {
                    _.Settings.ResolutionX = 1280;
                    _.Settings.ResolutionY = 720;
                }, _ => _.Settings.ResolutionX == 1280 && _.Settings.ResolutionY == 720, resolution1Button));
            _scene.AddComponent(resolution1Button, CreateButtonText("1280x720", 20));

            var resolution2Button= _scene.CreateEntity();
            _scene.AddComponent(resolution2Button, new PositionComponent { Position = new Vector3(581, -50, 0) });
            _scene.AddComponent(resolution2Button, new SizeComponent { Width = 98, Height = 40 });
            _scene.AddComponent(resolution2Button, new ColorComponent());
            _scene.AddComponent(resolution2Button, new SettingsScript(_settingsManager,
                _ =>
                {
                    _.Settings.ResolutionX = 1920;
                    _.Settings.ResolutionY = 1080;
                }, _ => _.Settings.ResolutionX == 1920 && _.Settings.ResolutionY == 1080, resolution2Button));
            _scene.AddComponent(resolution2Button, CreateButtonText("1920x1080", 20));

            var applyButton= _scene.CreateEntity();
            _scene.AddComponent(applyButton, new PositionComponent { Position = new Vector3(530, -100, 0) });
            _scene.AddComponent(applyButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(applyButton, new ColorComponent());
            _scene.AddComponent(applyButton, new SettingsScript(_settingsManager, _ => _.Apply(), _ => false, applyButton));
            _scene.AddComponent(applyButton, CreateButtonText("Apply"));

            var resetButton= _scene.CreateEntity();
            _scene.AddComponent(resetButton, new PositionComponent { Position = new Vector3(530, -150, 0) });
            _scene.AddComponent(resetButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(resetButton, new ColorComponent());
            _scene.AddComponent(resetButton, new SettingsScript(_settingsManager, _ => _.Reset(), _ => false, resetButton));
            _scene.AddComponent(resetButton, CreateButtonText("Reset"));
        }

        public bool Paused { get; }

        public void Update(float timeStep)
        {
            if (!_visible) return;

            _scene.Update(timeStep);
        }

        public void Render()
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
                    switch (keyPressed.KeyCode)
                    {
                        case KeyCode.Escape:
                            _visible = !_visible;
                            break;
                        case KeyCode.One:
                            _layerContainer.SetLayerOrder(typeof(ExampleLayer), typeof(SettingsLayer));
                            break;
                        case KeyCode.Two:
                            _layerContainer.SetLayerOrder(typeof(Layer2D), typeof(SettingsLayer));
                            break;
                        case KeyCode.Three:
                            _layerContainer.SetLayerOrder(typeof(PhysicsLayer), typeof(SettingsLayer));
                            break;
                        case KeyCode.Four:
                            _layerContainer.SetLayerOrder(typeof(TextLayer), typeof(SettingsLayer));
                            break;
                    }
                    break;
            }

            if (!_visible) return;

            _scene.HandleEvent(evnt);
        }

        private static TextComponent CreateButtonText(string label, uint size = 30)
        {
            return new TextComponent
            {
                Text = label,
                Font = "Assets/Roboto-Medium.ttf",
                Size = size,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            };
        }
    }
}
