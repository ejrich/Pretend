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
        private float _playerPosition = 450;
        private float _playerRotation;
        private float _velocity;

        public PlayerScript(PositionComponent position)
        {
            _position = position;
        }

        public void Update(float timeStep)
        {
            // Calculate delta y
            var deltaY = _velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            // Calculate next position
            _playerPosition = _playerPosition + deltaY > 0 ?//_floorHeight
                _playerPosition + deltaY : 0;//_floorHeight;

            // Recalculate velocity
            var deltaV = Gravity * timeStep;
            _velocity = _playerPosition > 0 ?//_floorHeight ?
                _velocity + deltaV : 0;

            // Flip the player object if it's in the air
            _playerRotation = _playerPosition > 0 ?//_floorHeight ?
                (_playerRotation - RotationSpeed * timeStep) % 360 : 0;

            _position.Y = _playerPosition;
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

        public ObstacleScript(PositionComponent position)
        {
            _position = position;
        }

        public void Update(float timeStep)
        {
            _position.X -= ObstacleSpeed * timeStep;
        }
    }
}
