using System;
using System.Collections.Generic;
using System.Linq;
using Pretend;

namespace Game
{
    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public interface IGame
    {
        List<Position> ObstaclePositions { get; }
        float PlayerPosition { get; }
        void Update(float timeStep);
        void Jump();
    }

    [Singleton]
    public class Game : IGame
    {
        private const float Speed = 100;
        private const float Gravity = -800;
        
        private readonly Random _random;
        private bool _jumping;
        private float _jumpTime;
        private bool _onObstacle;
        private float _floorHeight;
        private float _takeoffHeight;
        private float _jumpVelocity;

        public Game()
        {
            _random = new Random();
        }
        
        public List<Position> ObstaclePositions { get; private set; } = new List<Position>
        {
            new Position { X = 250 }, new Position { X = 450 }
        };
        public float PlayerPosition { get; private set; }

        public void Update(float timeStep)
        {
            Position currentObstacle = null;
            foreach (var obstaclePosition in ObstaclePositions)
            {
                obstaclePosition.X -= Speed * timeStep;

                if (obstaclePosition.X > -35 && obstaclePosition.X < 35)
                {
                    currentObstacle = obstaclePosition;
                }
            }

            var previousPosition = PlayerPosition;
            if (_jumping)
            {
                // if (_floorHeight > 0) { Console.WriteLine(); }
                _jumpTime += timeStep;
                var position = _takeoffHeight + _jumpVelocity * _jumpTime + -400 * _jumpTime * _jumpTime;

                PlayerPosition = position < _floorHeight ? _floorHeight : position;
                if (position <= _floorHeight)
                {
                    _jumpTime = 0;
                    _jumpVelocity = 0;
                    _jumping = false;
                }
            }
            if (currentObstacle != null)
            {
                _floorHeight = 35;
                if (previousPosition <= 0 || (previousPosition < PlayerPosition && previousPosition < 35))
                {
                    // Stop game here
                    Console.WriteLine($"Collision {currentObstacle.X}, {previousPosition}");
                }
                else if (previousPosition > 35 && PlayerPosition < 35)
                {
                    // _onObstacle = true;
                    // _jumpTime = 0;
                    Console.WriteLine($"On obstacle {currentObstacle.X}, {previousPosition}");
                }
            }
            else
            {
                _floorHeight = 0;
                // _onObstacle = false;
            }

            ObstaclePositions = ObstaclePositions.Where(pos => pos.X > -640).ToList();

            if (timeStep > 0 && //ObstaclePositions.Last().X < 620 &&
                _random.Next(Convert.ToInt32(1 / timeStep)) == 1) ObstaclePositions.Add(new Position { X = 640 });
        }

        public void Jump()
        {
            if (!_jumping)
            {
                _takeoffHeight = _floorHeight;
                _jumpVelocity = 300;
            }
            _jumping = true;
            _onObstacle = false;
        }
    }
}
