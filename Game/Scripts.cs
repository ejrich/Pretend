using System;
using System.Numerics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;

namespace Game
{
    public class PlayerScript : IScriptComponent
    {
        private const float JumpSpeed = 300;
        private const float RotationSpeed = 360;

        private readonly PhysicsComponent _physics;
        private readonly SourceComponent _source;

        public PlayerScript(IEntity playerEntity)
        {
            _physics = playerEntity.GetComponent<PhysicsComponent>();
            _source = playerEntity.GetComponent<SourceComponent>();
            _physics.AngularVelocity = new Vector3(0, 0, RotationSpeed);
        }

        public void Update(float timeStep)
        {
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    if (keyPressed.KeyCode == KeyCode.Space)
                    {
                        _physics.Velocity = new Vector3(0, JumpSpeed, 0);
                        _physics.AngularVelocity = new Vector3(0, 0, RotationSpeed);
                        _source.Play = true;
                    }
                    break;
            }
        }
    }

    public class SettingsScript : IScriptComponent
    {
        private readonly ISettingsManager<GameSettings> _settings;
        private readonly Action<ISettingsManager<GameSettings>> _set;
        private readonly Func<ISettingsManager<GameSettings>, bool> _active;
        private readonly ColorComponent _color;
        private readonly Vector2 _min;
        private readonly Vector2 _max;

        private static readonly Vector4 SelectedColor = new Vector4(0, 1, 1, 1);

        public SettingsScript(ISettingsManager<GameSettings> settings, Action<ISettingsManager<GameSettings>> set,
            Func<ISettingsManager<GameSettings>, bool> active, IEntity entity)
        {
            _settings = settings;
            _set = set;
            _active = active;
            _color = entity.GetComponent<ColorComponent>();

            var position = entity.GetComponent<PositionComponent>().Position;
            var size = entity.GetComponent<SizeComponent>();

            _min = new Vector2(position.X - size.Width / 2f, position.Y - size.Height / 2f);
            _max = new Vector2(position.X + size.Width / 2f, position.Y + size.Height / 2f);
        }

        public void Update(float timeStep)
        {
            if (_active(_settings))
            {
                if (_color.Color == Vector4.One)
                    _color.Color = SelectedColor;
            }
            else
                if (_color.Color == SelectedColor)
                    _color.Color = Vector4.One;
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case MouseButtonPressedEvent mousePressed:
                    HandleMousePress(mousePressed);
                    break;
            }
        }

        private void HandleMousePress(MouseButtonPressedEvent mousePressed)
        {
            if (_min.X > mousePressed.X || _max.X < mousePressed.X ||
                _min.Y > mousePressed.Y || _max.Y < mousePressed.Y)
                return;

            _set(_settings);
            mousePressed.Processed = true;
        }
    }
}
