using System;
using System.Reflection.Emit;
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
            var entities = entityContainer.GetEntitiesWithComponent<PhysicsComponent>();
            foreach (var entity in entities)
            {
                var physicsComponent = entity.GetComponent<PhysicsComponent>();
                if (physicsComponent.Fixed) continue;

                var positionComponent = entity.GetComponent<PositionComponent>();

                CalculatePosition(physicsComponent, positionComponent, timeStep);
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

        private Collision DetermineCollision(IEntity a, IEntity b)
        {
            var ap = a.GetComponent<PositionComponent>();
            var bp = b.GetComponent<PositionComponent>();
            var aSize = a.GetComponent<SizeComponent>();
            var bSize = b.GetComponent<SizeComponent>();
            var dx = Math.Abs(bp.X - ap.X) - (aSize.Width + bSize.Width);
            var dy = Math.Abs(bp.Y - ap.Y) - (aSize.Height + bSize.Height);

            if ((dx == 0 && dy > 0) || (dy == 0 && dx > 0)) return Collision.Touching;

            return dx < 0 && dy < 0 ? Collision.Colliding : Collision.Apart;
        }
    }
}
