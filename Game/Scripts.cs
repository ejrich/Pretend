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
}
