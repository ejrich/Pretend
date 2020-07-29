using System;
using System.Collections.Generic;
using System.Linq;
using Pretend;

namespace Game
{
    public interface IGame
    {
        List<float> ObstaclePositions { get; }
        void Update(float timeStep);
    }

    [Singleton]
    public class Game : IGame
    {
        private const float Speed = 200;
        private readonly Random _random;

        public Game()
        {
            _random = new Random();
        }
        
        public List<float> ObstaclePositions { get; private set; } = new List<float> { 250, 450 };

        public void Update(float timeStep)
        {
            ObstaclePositions = ObstaclePositions.Select(x => x - Speed * timeStep)
                .Where(x => x > -640).ToList();
            
            if (timeStep > 0 && ObstaclePositions.Last() < 620 &&
                _random.Next(Convert.ToInt32(1 / timeStep)) == 1) ObstaclePositions.Add(640);
        }
    }
}
