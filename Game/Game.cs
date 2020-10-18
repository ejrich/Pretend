using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Physics;

namespace Game
{
    public interface IGame
    {
        bool Running { get; }
        void Init(IScene scene, IPhysicsContainer physicsContainer, PositionComponent playerPosition);
        void Update(float timeStep);
        void Reset();
    }

    [Singleton]
    public class Game : IGame
    {
        private readonly Random _random;
        private readonly List<IEntity> _obstacles = new List<IEntity>();
        private IScene _scene;
        private IPhysicsContainer _physicsContainer;
        private PositionComponent _playerPosition;

        public Game() => _random = new Random();

        public bool Running { get; private set; } = true;
        private float _floorHeight;

        public void Init(IScene scene, IPhysicsContainer physicsContainer, PositionComponent playerPosition)
        {
            _scene = scene;
            _physicsContainer = physicsContainer;
            _playerPosition = playerPosition;

            ResetObstacles();

            _physicsContainer.Gravity = new Vector3(0, -800, 0);
            _physicsContainer.Start(144, _scene.EntityContainer);
        }

        private void AddObstacle(float x = 640)
        {
            var obstacle = _scene.CreateEntity();
            _obstacles.Add(obstacle);
            _scene.AddComponent(obstacle, new PositionComponent { X = x });
            _scene.AddComponent(obstacle, new SizeComponent { Width = 40, Height = 40 });
            _scene.AddComponent(obstacle, new PhysicsComponent { Fixed = true, Kinematic = true, Velocity = new Vector3(-200, 0, 0) });
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
            _floorHeight = 0;
            foreach (var obstacle in _obstacles)
            {
                var obstaclePosition = obstacle.GetComponent<PositionComponent>();
                if (obstaclePosition.X < -35 || obstaclePosition.X > 35) continue;

                _floorHeight = 35;
                break;
            }

            // Stop the game if there is a collision with an obstacle
            if (_floorHeight > 0 && _playerPosition.Y < _floorHeight)
            {
                Running = false;
                _physicsContainer.Stop();
                return;
            }

            // Filter the passed obstacles and determine whether to add a new one
            var furthestObstacle = _obstacles.FirstOrDefault();
            if (furthestObstacle?.GetComponent<PositionComponent>().X < -640)
                DeleteObstacle(furthestObstacle);

            if (timeStep > 0 && _random.Next(Convert.ToInt32(1 / timeStep)) == 1)
                AddObstacle();
        }

        public void Reset()
        {
            _floorHeight = 0;
            _playerPosition.Y = 450;
            Running = true;
            ResetObstacles();
            _physicsContainer.Start(144, _scene.EntityContainer);
        }
    }
}
