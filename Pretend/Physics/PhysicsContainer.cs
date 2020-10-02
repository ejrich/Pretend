using System;
using System.Collections.Generic;
using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public interface IPhysicsContainer
    {
        Vector3 Gravity { set; }
        int Iterations { set; }
        void Simulate(float timeStep, IEntityContainer entityContainer);
    }

    public class PhysicsContainer : IPhysicsContainer
    {
        public Vector3 Gravity { private get; set; }
        public int Iterations { private get; set; } = 4;

        public void Simulate(float timeStep, IEntityContainer entityContainer)
        {
            var dt = timeStep / Iterations;
            var entities = entityContainer.GetEntitiesWithComponent<PhysicsComponent>();
            for (var i = 0; i < Iterations; i++)
            {
                var newPositions = new Dictionary<IEntity, Vector3>();
                foreach (var entity in entities)
                {
                    var physicsComponent = entity.GetComponent<PhysicsComponent>();

                    var position = entity.GetComponent<PositionComponent>();
                    if (physicsComponent.Fixed)
                    {
                        newPositions.Add(entity, new Vector3(position.X, position.Y, position.Z));
                    }
                    else
                    {
                        var newPosition = CalculatePosition(physicsComponent, position, dt);
                        newPositions.Add(entity, newPosition);
                    }
                }
                foreach (var (entity, position) in newPositions)
                {
                    // Don't update if fixed
                    if (entity.GetComponent<PhysicsComponent>().Fixed) continue;

                    var updatePosition = true;
                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var otherPosition = newPositions[other];
                        var collision = DetermineCollision(entity, position, other, otherPosition);

                        if (collision != Collision.Apart) updatePosition = false;
                        if (collision == Collision.Colliding)
                        {
                            var newPosition = InterpolateCollision(entity, other, position, otherPosition);
                            ChangePosition(entity, newPosition);
                        }
                    }
                    if (!updatePosition) continue;

                    ChangePosition(entity, position);
                }
            }
        }

        private static Vector3 InterpolateCollision(IEntity entity, IEntity other, Vector3 position, Vector3 otherPosition)
        {
            var ePhysicsComponent = entity.GetComponent<PhysicsComponent>();

            // Don't try to move the position if the entity is fixed
            if (ePhysicsComponent.Fixed) return position;

            var (dx, dy) = CalculateDistance(position, entity.GetComponent<SizeComponent>(),
                otherPosition, other.GetComponent<SizeComponent>());
            var oPhysicsComponent = other.GetComponent<PhysicsComponent>();
            var eSize = entity.GetComponent<SizeComponent>();
            var oSize = other.GetComponent<SizeComponent>();
            var interpolatedPosition = new Vector3(position);

            // Simulate fixed collisions
            if (oPhysicsComponent.Fixed)
            {
                if (dx > dy)
                {
                    var dw = (eSize.Width / 2 + oSize.Width / 2) * (position.X > otherPosition.X ? 1 : -1);
                    interpolatedPosition.X = otherPosition.X + dw;
                }
                else
                {
                    var dh = (eSize.Height / 2 + oSize.Height / 2) * (position.Y > otherPosition.Y ? 1 : -1);
                    interpolatedPosition.Y = otherPosition.Y + dh;
                }
                ePhysicsComponent.Velocity = new Vector3(ePhysicsComponent.Velocity.X, 0, ePhysicsComponent.Velocity.Z);
            }
            // TODO Simulate elastics collisions

            return interpolatedPosition;
        }

        private Vector3 CalculatePosition(PhysicsComponent physicsComponent, PositionComponent position, float timeStep)
        {
            var acceleration = DetermineAcceleration(physicsComponent);
            
            // Recalculate velocity
            var deltaV = acceleration * timeStep;
            physicsComponent.Velocity += deltaV;

            // Calculate delta p
            var (x, y, z) = physicsComponent.Velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            // Calculate next position
            var newPosition = new Vector3(position.X + x, position.Y + y, position.Z + z);

            return newPosition;
        }

        private Vector3 DetermineAcceleration(PhysicsComponent physicsComponent)
        {
            if (physicsComponent.Force == default) return Gravity;

            var acceleration = Gravity + (physicsComponent.Force / (physicsComponent.Mass == 0 ? 1 : physicsComponent.Mass));
            physicsComponent.Force = default;

            return acceleration;
        }

        private static Collision DetermineCollision(IEntity a, Vector3 aNewPos, IEntity b, Vector3 bNewPos)
        {
            var (dx, dy) = CalculateDistance(aNewPos, a.GetComponent<SizeComponent>(), bNewPos, b.GetComponent<SizeComponent>());

            if (dx == 0 || dy == 0) return Collision.Touching;

            return dx < 0 && dy < 0 ? Collision.Colliding : Collision.Apart;
        }

        private static (float dx, float dy) CalculateDistance(Vector3 aPos, SizeComponent aSize, Vector3 bPos, SizeComponent bSize)
        {
            var dx = Math.Abs(bPos.X - aPos.X) - (aSize.Width / 2 + bSize.Width / 2);
            var dy = Math.Abs(bPos.Y - aPos.Y) - (aSize.Height / 2 + bSize.Height / 2);

            return (dx, dy);
        }

        private static void ChangePosition(IEntity entity, Vector3 position)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var (x, y, z) = position;
            positionComponent.X = x;
            positionComponent.Y = y;
            positionComponent.Z = z;
        }
    }
}
