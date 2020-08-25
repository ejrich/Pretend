using Pretend;
using Pretend.ECS;
using Pretend.Events;

namespace Game
{
    public class PlayerScript : IScriptComponent
    {
        private const float Gravity = -800;
        private const float RotationSpeed = 360;

        private readonly PositionComponent _position;
        private readonly IGame _game;

        private float _playerRotation;
        private float _velocity;

        public PlayerScript(PositionComponent position, IGame game)
        {
            _position = position;
            _game = game;
        }

        public void Update(float timeStep)
        {
            if (!_game.Running)
            {
                _velocity = 0;
                return;
            }

            // Calculate delta y
            var deltaY = _velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            // Calculate next position
            _position.Y = _position.Y + deltaY > _game.FloorHeight ?
                _position.Y + deltaY : _game.FloorHeight;

            // Recalculate velocity
            var deltaV = Gravity * timeStep;
            _velocity = _position.Y > _game.FloorHeight ?
                _velocity + deltaV : 0;

            // Flip the player object if it's in the air
            _playerRotation = _position.Y > _game.FloorHeight ?
                (_playerRotation - RotationSpeed * timeStep) % 360 : 0;

            _position.Rotation = _playerRotation;
        }

        public void HandleEvent(IEvent evnt)
        {
            _velocity = evnt switch
            {
                KeyPressedEvent keyPressed => keyPressed.KeyCode switch
                {
                    KeyCode.Space => 300,
                    _ => _velocity
                },
                _ => _velocity
            };
        }
    }

    public class ObstacleScript : IScriptComponent
    {
        private const float ObstacleSpeed = 200;

        private readonly PositionComponent _position;
        private readonly IGame _game;

        public ObstacleScript(PositionComponent position, IGame game)
        {
            _position = position;
            _game = game;
        }

        public void Update(float timeStep)
        {
            if (!_game.Running) return;

            _position.X -= ObstacleSpeed * timeStep;
        }
    }
}
