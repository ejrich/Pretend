using System;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public interface IPhysicsContainer
    {
        Vector3 Gravity { set; }
        void Simulate(float timeStep, IEntityContainer entityContainer);
    }

    public class PhysicsContainer : IPhysicsContainer
    {
        public Vector3 Gravity { private get; set; }

        public void Simulate(float timeStep, IEntityContainer entityContainer)
        {
            var dt = timeStep / 4;
            var entities = entityContainer.GetEntitiesWithComponent<PhysicsComponent>();
            for (var i = 0; i < 4; i++)
            {
                foreach (var entity in entities)
                {
                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var collision = DetermineCollision(entity, other);
                        Console.WriteLine(collision);
                        if (collision == Collision.Colliding)
                        {
                            // Console.WriteLine("Hello");
                        }
                    }
                    var physicsComponent = entity.GetComponent<PhysicsComponent>();
                    if (physicsComponent.Fixed) continue;

                    var positionComponent = entity.GetComponent<PositionComponent>();

                    CalculatePosition(physicsComponent, positionComponent, dt);
                }
            }
        }

        private void CalculatePosition(PhysicsComponent physicsComponent, PositionComponent position, float timeStep)
        {
            // Calculate delta p
            var (x, y, z) = physicsComponent.Velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            // Calculate next position
            position.X += x;
            position.Y += y;
            position.Z += z;

            // Recalculate velocity
            var deltaV = Gravity * timeStep;
            physicsComponent.Velocity += deltaV;
        }

        private static Collision DetermineCollision(IEntity a, IEntity b)
        {
            var aPos = a.GetComponent<PositionComponent>();
            var bPos = b.GetComponent<PositionComponent>();
            var aSize = a.GetComponent<SizeComponent>();
            var bSize = b.GetComponent<SizeComponent>();
            var dx = Math.Abs(bPos.X - aPos.X) - (aSize.Width / 2 + bSize.Width / 2);
            var dy = Math.Abs(bPos.Y - aPos.Y) - (aSize.Height / 2 + bSize.Height / 2);

            if ((dx == 0 && dy > 0) || (dy == 0 && dx > 0)) return Collision.Touching;

            return dx < 0 && dy < 0 ? Collision.Colliding : Collision.Apart;
        }
    }
}
