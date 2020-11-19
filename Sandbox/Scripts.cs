﻿using System;
using OpenTK.Mathematics;
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
        private readonly SourceComponent _source;

        private float _leftSpeed;
        private float _rightSpeed;
        private bool _jump;

        public ControlScript(PhysicsComponent physics, SourceComponent source)
        {
            _physics = physics;
            _source = source;
        }

        public void Update(float timeStep)
        {
            var xSpeed = _rightSpeed - _leftSpeed;

            if (_physics.Velocity.Y != 0) return;

            if (_jump)
            {
                _physics.Force = new Vector3(0, 100000, 0);
                _source.Play = true;
            }

            if (xSpeed == _physics.Velocity.X) return;
            _physics.Velocity = new Vector3(xSpeed, _physics.Velocity.Y, _physics.Velocity.Z);
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
                    _leftSpeed = 200;
                    break;
                case KeyCode.D:
                    _rightSpeed = 200;
                    break;
                case KeyCode.Space:
                    _jump = true;
                    break;
                case KeyCode.W:
                    _physics.AngularVelocity = new Vector3(0, 0, 90);
                    break;
                case KeyCode.X:
                    _physics.AngularVelocity = new Vector3(0, 0, -90);
                    break;
                case KeyCode.S:
                    _physics.AngularVelocity = Vector3.Zero;
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
                    _jump = false;
                    break;
            }
        }
    }

    public class SettingsScript : IScriptComponent
    {
        private readonly ISettingsManager<Settings> _settings;
        private readonly Action<ISettingsManager<Settings>> _set;
        private readonly Func<ISettingsManager<Settings>, bool> _active;
        private readonly ColorComponent _color;
        private readonly Vector2i _min;
        private readonly Vector2i _max;

        private static readonly Vector4 SelectedColor = new Vector4(0, 1, 1, 1);

        public SettingsScript(ISettingsManager<Settings> settings, Action<ISettingsManager<Settings>> set,
            Func<ISettingsManager<Settings>, bool> active, IEntity entity)
        {
            _settings = settings;
            _set = set;
            _active = active;
            _color = entity.GetComponent<ColorComponent>();

            var position = entity.GetComponent<PositionComponent>();
            var size = entity.GetComponent<SizeComponent>();

            _min = new Vector2i((int)position.X - (int)size.Width / 2, (int)position.Y - (int)size.Height / 2);
            _max = new Vector2i((int)position.X + (int)size.Width / 2, (int)position.Y + (int)size.Height / 2);
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
