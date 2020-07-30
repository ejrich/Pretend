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
        bool Running { get; }
        void Update(float timeStep);
        void Jump();
        void Reset();
    }

    [Singleton]
    public class Game : IGame
    {
        private const float Speed = 200;
        private const float Gravity = -800;

        private readonly Random _random;
        private float _floorHeight;
        private float _velocity;

        public Game() => _random = new Random();

        public List<Position> ObstaclePositions { get; private set; } = new List<Position>
        {
            new Position { X = 250 }, new Position { X = 450 }
        };
        public float PlayerPosition { get; private set; } = 450;
        public bool Running { get; private set; } = true;

        public void Update(float timeStep)
        {
            if (!Running) return;

            var obstacleInRange = false;
            foreach (var obstaclePosition in ObstaclePositions)
            {
                obstaclePosition.X -= Speed * timeStep;

                if (obstaclePosition.X > -35 && obstaclePosition.X < 35)
                {
                    _floorHeight = 35;
                    obstacleInRange = true;
                }
            }

            if (!obstacleInRange) _floorHeight = 0;

            // Calculate delta y
            var deltaY = _velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            if (obstacleInRange && PlayerPosition < _floorHeight)
            {
                Running = false;
                return;
            }

            // Calculate d1
            PlayerPosition = PlayerPosition + deltaY > _floorHeight ? PlayerPosition + deltaY : _floorHeight;

            // Recalculate velocity
            var deltaV = Gravity * timeStep;
            _velocity = PlayerPosition > _floorHeight ? _velocity + deltaV : 0;

            ObstaclePositions = ObstaclePositions.Where(pos => pos.X > -640).ToList();

            if (timeStep > 0 && ObstaclePositions.Last().X < 620 &&
                _random.Next(Convert.ToInt32(1 / timeStep)) == 1) ObstaclePositions.Add(new Position {X = 640});
        }

        public void Jump()
        {
            _velocity = 300;
        }

        public void Reset()
        {
            _velocity = 0;
            _floorHeight = 0;
            PlayerPosition = 450;
            ObstaclePositions = new List<Position> {new Position {X = 250}, new Position {X = 450}};
            Running = true;
        }
    }
}
