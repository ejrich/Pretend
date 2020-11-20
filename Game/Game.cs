using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using Pretend;
using Pretend.ECS;
using Pretend.Physics;
using Pretend.Text;

namespace Game
{
    public interface IGame
    {
        bool Running { get; }
        void Init(IScene scene, IPhysicsContainer physicsContainer, IEntity player, IEntity theme);
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
        private IEntity _player;
        private PositionComponent _playerPosition;
        private SourceComponent _themeSource;
        private TextComponent _title;
        private TextComponent _scoreText;
        private float _score;

        public Game() => _random = new Random();

        public bool Running { get; private set; }

        public void Init(IScene scene, IPhysicsContainer physicsContainer, IEntity player, IEntity theme)
        {
            _scene = scene;
            _physicsContainer = physicsContainer;
            _player = player;
            _playerPosition = player.GetComponent<PositionComponent>();
            _themeSource = theme.GetComponent<SourceComponent>();

            var titleEntity = _scene.CreateEntity();
            _title = new TextComponent
            {
                Text = "Possible Game\n\nPress Enter to Start,\nSpace to Jump",
                Font = "Assets/Roboto-Thin.ttf",
                Size = 60,
                RelativePosition = new Vector3(0, 300, 0)
            };
            _scene.AddComponent(titleEntity, _title);

            var scoreEntity = _scene.CreateEntity();
            _scoreText = new TextComponent
            {
                Font = "Assets/Roboto-Thin.ttf",
                Size = 45,
                Alignment = TextAlignment.Left,
                RelativePosition = new Vector3(-620, 300, 0)
            };
            _scene.AddComponent(scoreEntity, _scoreText);

            ResetObstacles();

            _physicsContainer.Gravity = new Vector3(0, -800, 0);
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
            _scoreText.Text = $"Score: {(int)_score}";
            if (!Running) return;

            // Recalculate obstacle positions and determine the floor height
            foreach (var obstacle in _obstacles)
            {
                var obstaclePosition = obstacle.GetComponent<PositionComponent>();
                if (obstaclePosition.X < -35 || obstaclePosition.X > 35) continue;

                // Stop the game if there is a collision with an obstacle
                var gjkResult = Algorithms.GJK(_player, obstacle);
                if (!gjkResult.Collision) continue;

                var penetrationVector = Algorithms.EPA(gjkResult);
                if (penetrationVector.X <= 1e-7) continue;

                Running = false;
                _title.Text = "Press Enter to Restart";
                _physicsContainer.Stop();
                _themeSource.Source.Stop();
                return;
            }

            _score += timeStep * 10;

            // Filter the passed obstacles and determine whether to add a new one
            var furthestObstacle = _obstacles.FirstOrDefault();
            if (furthestObstacle?.GetComponent<PositionComponent>().X < -640)
                DeleteObstacle(furthestObstacle);

            if (timeStep > 0 && _random.Next(Convert.ToInt32(1 / timeStep)) == 1)
                AddObstacle();
        }

        public void Reset()
        {
            _playerPosition.X = 0;
            _playerPosition.Y = 450;
            Running = true;
            ResetObstacles();
            _physicsContainer.Start(144, _scene.EntityContainer);
            _themeSource.Play = true;
            _title.Text = string.Empty;
            _score = 0;
        }
    }
}
