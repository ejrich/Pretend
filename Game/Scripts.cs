using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;

namespace Game
{
    public class PlayerScript : IScriptComponent
    {
        private const float RotationSpeed = 360;

        private readonly PositionComponent _position;
        private readonly PhysicsComponent _physics;
        private readonly IGame _game;

        public PlayerScript(PositionComponent position, PhysicsComponent physics, IGame game)
        {
            _position = position;
            _physics = physics;
            _game = game;
        }

        public void Update(float timeStep)
        {
            _physics.Fixed = !_game.Running;
            if (!_game.Running)
            {
                _physics.Velocity = Vector3.Zero;
                return;
            }

            // Flip the player object if it's in the air
            _position.Yaw = _position.Y > _game.FloorHeight ?
                (_position.Yaw - RotationSpeed * timeStep) % 360 : 0;
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    if (keyPressed.KeyCode == KeyCode.Space)
                    {
                        _physics.Velocity = new Vector3(0, 300, 0);
                        _physics.Fixed = false;
                    }
                    break;
            }
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
