using System;
using System.Collections.Generic;
using System.Linq;
using Pretend;
using Pretend.ECS;

namespace Game
{
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
        private readonly Random _random;
        private readonly List<IEntity> _obstacles = new List<IEntity>();
        private IScene _scene;
        private PositionComponent _playerPosition;

        public Game() => _random = new Random();

        public bool Running { get; private set; } = true;
        public float FloorHeight { get; private set; }

        public void Init(IScene scene, PositionComponent playerPosition)
        {
            _scene = scene;
            _playerPosition = playerPosition;

            ResetObstacles();
        }

        private void AddObstacle(float x = 640)
        {
            var obstacle = _scene.CreateEntity();
            _obstacles.Add(obstacle);
            var obstaclePosition = new PositionComponent { X = x };
            _scene.AddComponent(obstacle, obstaclePosition);
            _scene.AddComponent(obstacle, new SizeComponent { Width = 40, Height = 40 });
            _scene.AddComponent(obstacle, new ObstacleScript(obstaclePosition, this));
        }

        private void DeleteObstacle(IEntity obstacle)
        {
            _scene.DeleteEntity(obstacle);
            _obstacles.Remove(obstacle);
        }

        private void ResetObstacles()
        {
            foreach (var obstacle in _obstacles)
            {
                _scene.DeleteEntity(obstacle);
            }
            _obstacles.Clear();
            AddObstacle(250);
            AddObstacle(450);
        }

        public void Update(float timeStep)
        {
            if (!Running) return;

            // Recalculate obstacle positions and determine the floor height
            FloorHeight = 0;
            foreach (var obstacle in _obstacles)
            {
                var obstaclePosition = obstacle.GetComponent<PositionComponent>();
                if (obstaclePosition.X < -35 || obstaclePosition.X > 35) continue;

                FloorHeight = 35;
                break;
            }

            // Stop the game if there is a collision with an obstacle
            if (FloorHeight > 0 && _playerPosition.Y < FloorHeight)
            {
                Running = false;
                return;
            }

            // Filter the passed obstacles and determine whether to add a new one
            var furthestObstacle = _obstacles.First();
            if (furthestObstacle.GetComponent<PositionComponent>().X < -640)
                DeleteObstacle(furthestObstacle);

            if (timeStep > 0 && _random.Next(Convert.ToInt32(1 / timeStep)) == 1)
                AddObstacle();
        }

        public void Reset()
        {
            FloorHeight = 0;
            _playerPosition.Y = 450;
            Running = true;
            ResetObstacles();
        }
    }
}
