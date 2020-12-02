using System;
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
        private readonly IFactory _factory;

        private Settings Settings => _settingsManager.Settings;
        private bool _visible;

        public SettingsLayer(ICamera camera, IScene scene, ISettingsManager<Settings> settingsManager,
            ILayerContainer layerContainer, IFactory factory)
        {
            _camera = camera;
            _scene = scene;
            _settingsManager = settingsManager;
            _layerContainer = layerContainer;
            _factory = factory;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var fullscreenButton = _factory.Create<IButton>();
            fullscreenButton.Init(_scene, CreateButtonSettings("Fullscreen", y: 150));
            fullscreenButton.OnRelease = () =>
            {
                if (Settings.WindowMode.HasFlag(WindowMode.Fullscreen))
                    Settings.WindowMode &= ~WindowMode.Fullscreen;
                else
                    Settings.WindowMode |= WindowMode.Fullscreen;
            };
            fullscreenButton.OnUpdate = SetActive(() => Settings.WindowMode.HasFlag(WindowMode.Fullscreen));
            fullscreenButton.OnMouseOver = () => Console.WriteLine("Mouse Over Fullscreen Button");
            fullscreenButton.OnMouseLeave = () => Console.WriteLine("Mouse Left Fullscreen Button");

            var borderlessButton = _factory.Create<IButton>();
            borderlessButton.Init(_scene, CreateButtonSettings("Borderless", y: 100));
            borderlessButton.OnRelease = () =>
            {
                if (Settings.WindowMode.HasFlag(WindowMode.Borderless))
                    Settings.WindowMode &= ~WindowMode.Borderless;
                else
                    Settings.WindowMode |= WindowMode.Borderless;
            };
            borderlessButton.OnUpdate = SetActive(() => Settings.WindowMode.HasFlag(WindowMode.Borderless));

            var vsyncButton = _factory.Create<IButton>();
            vsyncButton.Init(_scene, CreateButtonSettings("Vsync", y: 50));
            vsyncButton.OnRelease = () => Settings.Vsync = !Settings.Vsync;
            vsyncButton.OnUpdate = SetActive(() => Settings.Vsync);

            var zeroFpsButton = _factory.Create<IButton>();
            zeroFpsButton.Init(_scene, CreateButtonSettings("0", x: 460, width: 60));
            zeroFpsButton.OnRelease = () => Settings.MaxFps = 0;
            zeroFpsButton.OnUpdate = SetActive(() => Settings.MaxFps == 0);

            var sixtyFpsButton = _factory.Create<IButton>();
            sixtyFpsButton.Init(_scene, CreateButtonSettings("60", width: 60));
            sixtyFpsButton.OnRelease = () => Settings.MaxFps = 60;
            sixtyFpsButton.OnUpdate = SetActive(() => Settings.MaxFps == 60);

            var highFpsButton = _factory.Create<IButton>();
            highFpsButton.Init(_scene, CreateButtonSettings("144", x: 600, width: 60));
            highFpsButton.OnRelease = () => Settings.MaxFps = 144;
            highFpsButton.OnUpdate = SetActive(() => Settings.MaxFps == 144);

            var resolution1Button = _factory.Create<IButton>();
            resolution1Button.Init(_scene, CreateButtonSettings("1280x720", 20, 479, -50, 98));
            resolution1Button.OnRelease = () => { Settings.ResolutionX = 1280; Settings.ResolutionY = 720; };
            resolution1Button.OnUpdate = SetActive(() => Settings.ResolutionX == 1280 && Settings.ResolutionY == 720);

            var resolution2Button = _factory.Create<IButton>();
            resolution2Button.Init(_scene, CreateButtonSettings("1920x1080", 20, 581, -50, 98));
            resolution2Button.OnRelease = () => { Settings.ResolutionX = 1920; Settings.ResolutionY = 1080; };
            resolution2Button.OnUpdate = SetActive(() => Settings.ResolutionX == 1920 && Settings.ResolutionY == 1080);

            var applyButton = _factory.Create<IButton>();
            applyButton.Init(_scene, CreateButtonSettings("Apply", y: -100));
            applyButton.OnRelease = () => _settingsManager.Apply();

            var resetButton = _factory.Create<IButton>();
            resetButton.Init(_scene, CreateButtonSettings("Reset", y: -150));
            resetButton.OnRelease = () => _settingsManager.Reset();
        }

        public bool Paused => !_visible;

        public void Update(float timeStep)
        {
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

        private static ButtonSettings CreateButtonSettings(string label, uint size = 30, int x = 530, int y = 0, int width = 200, int height = 40)
        {
            return new ButtonSettings
            {
                Text = label,
                Font = "Assets/Roboto-Medium.ttf",
                FontSize = size,
                FontColor = new Vector4(0, 0, 0, 1),
                Position = new Vector3(x, y, 0),
                Size = new Vector2(width, height)
            };
        }

        private static readonly Vector4 SelectedColor = new Vector4(0, 1, 1, 1);

        private static Action<ButtonSettings> SetActive(Func<bool> active)
        {
            return buttonSettings =>
            {
                if (active())
                {
                    if (buttonSettings.Color == Vector4.One)
                        buttonSettings.Color = SelectedColor;
                }
                else if (buttonSettings.Color == SelectedColor)
                    buttonSettings.Color = Vector4.One;
            };
        }
    }
}
