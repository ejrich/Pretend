﻿using OpenTK.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Game
{
    public class SettingsLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly ISettingsManager<GameSettings> _settingsManager;
        private readonly IGame _game;

        private bool _visible;

        public SettingsLayer(ICamera camera, IScene scene, ISettingsManager<GameSettings> settingsManager, IGame game)
        {
            _camera = camera;
            _scene = scene;
            _settingsManager = settingsManager;
            _game = game;
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var settingsBackground= _scene.CreateEntity();
            _scene.AddComponent(settingsBackground, new PositionComponent { X = 530, Y = 25, Z = 0.01f });
            _scene.AddComponent(settingsBackground, new SizeComponent { Width = 210, Height = 400 });
            _scene.AddComponent(settingsBackground, new ColorComponent { Color = new Vector4(0.1f, 0.1f, 0.1f, 1) });
            
            var musicButton= _scene.CreateEntity();
            _scene.AddComponent(musicButton, new PositionComponent { X = 479, Y = 200, Z = 0.02f });
            _scene.AddComponent(musicButton, new SizeComponent { Width = 98, Height = 40 });
            _scene.AddComponent(musicButton, new ColorComponent());
            _scene.AddComponent(musicButton, new SettingsScript(_settingsManager,
                _ => _.Settings.Music = !_.Settings.Music, _ => _.Settings.Music, musicButton));
            _scene.AddComponent(musicButton, CreateButtonText("Music"));

            var soundEffectsButton= _scene.CreateEntity();
            _scene.AddComponent(soundEffectsButton, new PositionComponent { X = 581, Y = 200, Z = 0.02f });
            _scene.AddComponent(soundEffectsButton, new SizeComponent { Width = 98, Height = 40 });
            _scene.AddComponent(soundEffectsButton, new ColorComponent());
            _scene.AddComponent(soundEffectsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.SoundEffects = !_.Settings.SoundEffects, _ => _.Settings.SoundEffects, soundEffectsButton));
            _scene.AddComponent(soundEffectsButton, CreateButtonText("Effects"));

            var fullscreenButton= _scene.CreateEntity();
            _scene.AddComponent(fullscreenButton, new PositionComponent { X = 530, Y = 150, Z = 0.02f });
            _scene.AddComponent(fullscreenButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(fullscreenButton, new ColorComponent());
            _scene.AddComponent(fullscreenButton, new SettingsScript(_settingsManager,
                _ =>
                {
                    if (_.Settings.WindowMode.HasFlag(WindowMode.Fullscreen))
                        _.Settings.WindowMode &= ~WindowMode.Fullscreen;
                    else
                        _.Settings.WindowMode |= WindowMode.Fullscreen;
                }, _ => _.Settings.WindowMode.HasFlag(WindowMode.Fullscreen), fullscreenButton));
            _scene.AddComponent(fullscreenButton, CreateButtonText("Fullscreen"));

            var borderlessButton= _scene.CreateEntity();
            _scene.AddComponent(borderlessButton, new PositionComponent { X = 530, Y = 100, Z = 0.02f });
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
            _scene.AddComponent(vsyncButton, new PositionComponent { X = 530, Y = 50, Z = 0.02f });
            _scene.AddComponent(vsyncButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(vsyncButton, new ColorComponent());
            _scene.AddComponent(vsyncButton, new SettingsScript(_settingsManager,
                _ => _.Settings.Vsync = !_.Settings.Vsync, _ => _.Settings.Vsync, vsyncButton));
            _scene.AddComponent(vsyncButton, CreateButtonText("Vsync"));

            var zeroFpsButton= _scene.CreateEntity();
            _scene.AddComponent(zeroFpsButton, new PositionComponent { X = 460, Z = 0.02f });
            _scene.AddComponent(zeroFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(zeroFpsButton, new ColorComponent());
            _scene.AddComponent(zeroFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 0, _ => _.Settings.MaxFps == 0, zeroFpsButton));
            _scene.AddComponent(zeroFpsButton, CreateButtonText("0"));

            var sixtyFpsButton= _scene.CreateEntity();
            _scene.AddComponent(sixtyFpsButton, new PositionComponent { X = 530, Z = 0.02f });
            _scene.AddComponent(sixtyFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(sixtyFpsButton, new ColorComponent());
            _scene.AddComponent(sixtyFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 60, _ => _.Settings.MaxFps == 60, sixtyFpsButton));
            _scene.AddComponent(sixtyFpsButton, CreateButtonText("60"));

            var highFpsButton= _scene.CreateEntity();
            _scene.AddComponent(highFpsButton, new PositionComponent { X = 600, Z = 0.02f });
            _scene.AddComponent(highFpsButton, new SizeComponent { Width = 60, Height = 40 });
            _scene.AddComponent(highFpsButton, new ColorComponent());
            _scene.AddComponent(highFpsButton, new SettingsScript(_settingsManager,
                _ => _.Settings.MaxFps = 144, _ => _.Settings.MaxFps == 144, highFpsButton));
            _scene.AddComponent(highFpsButton, CreateButtonText("144"));

            var resolution1Button= _scene.CreateEntity();
            _scene.AddComponent(resolution1Button, new PositionComponent { X = 479, Y = -50, Z = 0.02f });
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
            _scene.AddComponent(resolution2Button, new PositionComponent { X = 581, Y = -50, Z = 0.02f });
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
            _scene.AddComponent(applyButton, new PositionComponent { X = 530, Y = -100, Z = 0.02f });
            _scene.AddComponent(applyButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(applyButton, new ColorComponent());
            _scene.AddComponent(applyButton, new SettingsScript(_settingsManager,
                _ => _.Apply(s =>
                {
                    _game.Music = s.Music;
                    _game.SoundEffects = s.SoundEffects;
                }), _ => false, applyButton));
            _scene.AddComponent(applyButton, CreateButtonText("Apply"));

            var resetButton= _scene.CreateEntity();
            _scene.AddComponent(resetButton, new PositionComponent { X = 530, Y = -150, Z = 0.02f });
            _scene.AddComponent(resetButton, new SizeComponent { Width = 200, Height = 40 });
            _scene.AddComponent(resetButton, new ColorComponent());
            _scene.AddComponent(resetButton, new SettingsScript(_settingsManager, _ => _.Reset(), _ => false, resetButton));
            _scene.AddComponent(resetButton, CreateButtonText("Reset"));
        }

        public void Update(float timeStep)
        {
            if (!_visible) return;

            _scene.Update(timeStep);
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

        private static TextComponent CreateButtonText(string label, uint size = 30)
        {
            return new TextComponent
            {
                Text = label,
                Font = "Assets/Roboto-Thin.ttf",
                Size = size,
                RelativePosition = new Vector3(0, -2.5f, 0.01f),
                Color = new Vector4(0, 0, 0, 1)
            };
        }
    }
}
