using System.Numerics;
using Pretend;
using Pretend.Audio;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;
using Pretend.Physics;

namespace Game
{
    public class GameLayer : ILayer
    {
        private readonly ICamera _camera;
        private readonly IScene _scene;
        private readonly IPhysicsContainer _physicsContainer;
        private readonly ISoundManager _soundManager;
        private readonly IGame _game;
        private readonly ILayerContainer _layerContainer;
        private readonly GameSettings _gameSettings;

        public GameLayer(ICamera camera, IScene scene, IPhysicsContainer physicsContainer, ISoundManager soundManager,
            IGame game, ILayerContainer layerContainer, IEventDispatcher eventDispatcher, Settings gameSettings)
        {
            _camera = camera;
            _scene = scene;
            _physicsContainer = physicsContainer;
            _soundManager = soundManager;
            _game = game;
            _layerContainer = layerContainer;
            _gameSettings = (GameSettings)gameSettings;

            eventDispatcher.Register<GameResumeEvent>(_ => Resume());
        }

        public void Attach()
        {
            _scene.Init();

            var cameraEntity = _scene.CreateEntity();
            _scene.AddComponent(cameraEntity, new CameraComponent { Camera = _camera, Active = true });

            var jumpSound = _soundManager.CreateSoundBuffer();
            jumpSound.SetData("Assets/jump.wav");
            var playerSource = _soundManager.CreateSource();

            var playerEntity = _scene.CreateEntity();
            _scene.AddComponent(playerEntity, new PositionComponent { Position = new Vector3(0, 450, 0) });
            _scene.AddComponent(playerEntity, new SizeComponent { Width = 30, Height = 30 });
            _scene.AddComponent(playerEntity, new ColorComponent { Color = new Vector4(1, 0, 0, 1) });
            _scene.AddComponent(playerEntity, new PhysicsComponent());
            _scene.AddComponent(playerEntity, new SourceComponent { Source = playerSource, SoundBuffer = jumpSound });
            _scene.AddComponent(playerEntity, new PlayerScript(playerEntity));

            var floorEntity = _scene.CreateEntity();
            _scene.AddComponent(floorEntity, new PositionComponent { Position = new Vector3(0, -25, 0) });
            _scene.AddComponent(floorEntity, new SizeComponent { Width = 1280, Height = 20 });
            _scene.AddComponent(floorEntity, new PhysicsComponent { Fixed = true });

            var theme= _soundManager.CreateSoundBuffer();
            theme.SetData("Assets/theme.wav");
            var themeSource = _soundManager.CreateSource();

            var themeEntity = _scene.CreateEntity();
            _scene.AddComponent(themeEntity, new SourceComponent { Source = themeSource, SoundBuffer = theme, Play = true, Loop = true });

            _game.Init(_scene, _physicsContainer, playerEntity, themeEntity);
            _game.Music = _gameSettings.Music;
            _game.SoundEffects = _gameSettings.SoundEffects;
            _soundManager.Start(60, _scene.EntityContainer);
        }

        public void Pause()
        {
            _physicsContainer.Stop();
            Paused = true;
        }

        public void Resume()
        {
            if (_game.Running)
                _physicsContainer.Start(144, _scene.EntityContainer);
            Paused = false;
        }

        public bool Paused { get; private set; }

        public void Update(float timeStep)
        {
            // Update the game
            _game.Update(timeStep);

            _scene.Update(timeStep);
        }

        public void Render()
        {
            _scene.Render();
        }

        public void Detach()
        {
            _physicsContainer.Stop();
            _soundManager.Stop();
            _soundManager.Dispose();
        }

        public void HandleEvent(IEvent evnt)
        {
            _scene.HandleEvent(evnt);
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
                    break;
                case WindowResizeEvent resize:
                    _camera.Resize(resize.Width, resize.Height);
                    break;
            }
        }

        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.Enter:
                    if (!_game.Running) _game.Reset();
                    break;
                case KeyCode.Escape:
                    _layerContainer.PushLayer<SettingsLayer>();
                    Pause();
                    break;
            }
        }
    }
}
