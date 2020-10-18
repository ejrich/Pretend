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

        public PlayerScript(PositionComponent position, PhysicsComponent physics)
        {
            _position = position;
            _physics = physics;
        }

        public void Update(float timeStep)
        {
            // Flip the player object if it's in the air
            _position.Yaw = _physics.Velocity.Y != 0 ?
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
                    }
                    break;
            }
        }
    }
}
