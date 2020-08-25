using System;
using System.Collections.Generic;
using System.Linq;
using Pretend;
using Pretend.ECS;

namespace Game
{
    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public interface IGame
    {
        bool Running { get; }
        float FloorHeight { get; }
        void Init(IScene scene, PositionComponent playerPosition);
        void Update(float timeStep);
        void Reset();
    }

    [Singleton]
    public class Game : IGame
    {
        private const float ObstacleSpeed = 200;
        
        private readonly Random _random;
        private List<Position> _obstaclePositions = new List<Position>();
        private IScene _scene;
        private PositionComponent _playerPosition;

        public Game() => _random = new Random();

        public bool Running { get; private set; } = true;
        public float FloorHeight { get; private set; }

        public void Init(IScene scene, PositionComponent playerPosition)
        {
            _scene = scene;
            _playerPosition = playerPosition;

            AddObstacle(250);
            AddObstacle(450);
        }

        private void AddObstacle(float x = 640)
        {
            _obstaclePositions.Add(new Position { X = x });
            var obstacle = _scene.CreateEntity();
            var obstaclePosition = new PositionComponent { X = x };
            _scene.AddComponent(obstacle, obstaclePosition);
            _scene.AddComponent(obstacle, new SizeComponent { Width = 40, Height = 40 });
            _scene.AddComponent<IScriptComponent>(obstacle, new ObstacleScript(obstaclePosition, this));
        }

        public void Update(float timeStep)
        {
            if (!Running) return;

            // Recalculate obstacle positions and determine the floor height
            FloorHeight = 0;
            foreach (var obstaclePosition in _obstaclePositions)
            {
                obstaclePosition.X -= ObstacleSpeed * timeStep;
                if (obstaclePosition.X > -35 && obstaclePosition.X < 35)
                {
                    FloorHeight = 35;
                }
            }

            // Stop the game if there is a collision with an obstacle
            if (FloorHeight > 0 && _playerPosition.Y < FloorHeight)
            {
                Running = false;
                return;
            }

            // Filter the passed obstacles and determine whether to add a new one
            _obstaclePositions = _obstaclePositions.Where(pos => pos.X > -640).ToList();
            if (timeStep > 0 && _random.Next(Convert.ToInt32(1 / timeStep)) == 1)
                AddObstacle();
        }

        public void Reset()
        {
            FloorHeight = 0;
            _playerPosition.Y = 450;
            Running = true;
        }
    }
}
