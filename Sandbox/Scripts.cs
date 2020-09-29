using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;

namespace Sandbox
{
    public class CameraScript : IScriptComponent 
    {
        private readonly ICamera _camera;
        
        private Vector3 _position;
        private float _leftSpeed;
        private float _rightSpeed;
        private float _upSpeed;
        private float _downSpeed;
        
        public CameraScript(ICamera camera)
        {
            _camera = camera;
            _position = camera.Position;
        }

        public void Update(float timeStep)
        {
            // Calculate location by speed
            var xSpeed = _rightSpeed - _leftSpeed;
            var ySpeed = _upSpeed - _downSpeed;

            _position.X += xSpeed * timeStep;
            _position.Y += ySpeed * timeStep;

            _camera.Position = _position;
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
                    break;
                case KeyReleasedEvent keyReleased:
                    HandleKeyRelease(keyReleased);
                    break;
            }
        }
        
        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _upSpeed = 1;
                    break;
                case KeyCode.S:
                    _downSpeed = 1;
                    break;
                case KeyCode.A:
                    _leftSpeed = 1;
                    break;
                case KeyCode.D:
                    _rightSpeed = 1;
                    break;
            }
        }

        private void HandleKeyRelease(KeyReleasedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.W:
                    _upSpeed = 0;
                    break;
                case KeyCode.S:
                    _downSpeed = 0;
                    break;
                case KeyCode.A:
                    _leftSpeed = 0;
                    break;
                case KeyCode.D:
                    _rightSpeed = 0;
                    break;
            }
        }
    }

    public class DiceScript : IScriptComponent
    {
        private readonly PositionComponent _position;

        public DiceScript(PositionComponent position)
        {
            _position = position;
        }
        
        public void Update(float timeStep)
        {
            _position.Yaw += 100 * timeStep;
            if (_position.Yaw >= 360)
                _position.Yaw = 0;
        }
    }

    public class ControlScript : IScriptComponent
    {
        private readonly PhysicsComponent _physics;

        private float _leftSpeed;
        private float _rightSpeed;
        private float _upSpeed;

        public ControlScript(PhysicsComponent physics)
        {
            _physics = physics;
        }
        
        public void Update(float timeStep)
        {
            var xSpeed = _rightSpeed - _leftSpeed;
            var ySpeed = _physics.Velocity.Y == 0 ? _upSpeed : _physics.Velocity.Y;

            if (xSpeed == _physics.Velocity.X && ySpeed == _physics.Velocity.Y) return;
            _physics.Velocity = new Vector3(xSpeed, ySpeed, _physics.Velocity.Z);
        }

        public void HandleEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case KeyPressedEvent keyPressed:
                    HandleKeyPress(keyPressed);
                    break;
                case KeyReleasedEvent keyReleased:
                    HandleKeyRelease(keyReleased);
                    break;
            }
        }

        private void HandleKeyPress(KeyPressedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.A:
                    _leftSpeed = 100;
                    break;
                case KeyCode.D:
                    _rightSpeed = 100;
                    break;
                case KeyCode.Space:
                    _upSpeed = 200;
                    break;
            }
        }

        private void HandleKeyRelease(KeyReleasedEvent evnt)
        {
            switch (evnt.KeyCode)
            {
                case KeyCode.A:
                    _leftSpeed = 0;
                    break;
                case KeyCode.D:
                    _rightSpeed = 0;
                    break;
                case KeyCode.Space:
                    _upSpeed = 0;
                    break;
            }
        }
    }
}
