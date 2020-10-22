using OpenTK.Mathematics;
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

        public PlayerScript(PhysicsComponent physics)
        {
            _physics = physics;
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
                    }
                    break;
            }
        }
    }
}
