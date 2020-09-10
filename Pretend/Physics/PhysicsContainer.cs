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
                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var collision = DetermineCollision(entity, other);
                        Console.WriteLine(collision);
                        if (collision == Collision.Colliding)
                        {
                            var otherPosition = newPositions[other];
                            InterpolateCollision(entity, other, position, otherPosition);
                        }
                    }

                    var positionComponent = entity.GetComponent<PositionComponent>();
                    positionComponent.X = position.X;
                    positionComponent.Y = position.Y;
                    positionComponent.Z = position.Z;
                }
            }
        }

        private void InterpolateCollision(IEntity entity, IEntity other, Vector3 position, Vector3 otherPosition)
        {
            var (dx, dy) = CalculateDistance(entity, other);
            var eFixed = entity.GetComponent<PhysicsComponent>().Fixed;
            var oFixed = other.GetComponent<PhysicsComponent>().Fixed;

            if (eFixed || oFixed)
            {
                entity.GetComponent<PhysicsComponent>().Velocity = Vector3.Zero;
                other.GetComponent<PhysicsComponent>().Velocity = Vector3.Zero;
            }
        }

        private Vector3 CalculatePosition(PhysicsComponent physicsComponent, PositionComponent position, float timeStep)
        {
            // Calculate delta p
            var (x, y, z) = physicsComponent.Velocity * timeStep + 0.5f * Gravity * timeStep * timeStep;

            // Calculate next position
            var newPosition = new Vector3(position.X += x, position.Y += y, position.Z += z);
            // position.X += x;
            // position.Y += y;
            // position.Z += z;

            // Recalculate velocity
            var deltaV = Gravity * timeStep;
            physicsComponent.Velocity += deltaV;

            return newPosition;
        }

        private static Collision DetermineCollision(IEntity a, IEntity b)
        {
            var (dx, dy) = CalculateDistance(a, b);

            if ((dx == 0 && dy > 0) || (dy == 0 && dx > 0)) return Collision.Touching;

            return dx < 0 && dy < 0 ? Collision.Colliding : Collision.Apart;
        }

        private static (float dx, float dy) CalculateDistance(IEntity a, IEntity b)
        {
            var aPos = a.GetComponent<PositionComponent>();
            var bPos = b.GetComponent<PositionComponent>();
            var aSize = a.GetComponent<SizeComponent>();
            var bSize = b.GetComponent<SizeComponent>();
            var dx = Math.Abs(bPos.X - aPos.X) - (aSize.Width / 2 + bSize.Width / 2);
            var dy = Math.Abs(bPos.Y - aPos.Y) - (aSize.Height / 2 + bSize.Height / 2);

            return (dx, dy);
        }
    }
}
