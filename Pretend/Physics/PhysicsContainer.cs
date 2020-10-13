using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenToolkit.Mathematics;
using Pretend.ECS;

namespace Pretend.Physics
{
    public interface IPhysicsContainer
    {
        Vector3 Gravity { set; }
        int Iterations { set; }
        bool Running { get; }
        void Simulate(float timeStep, IEntityContainer entityContainer);
        void Start(int hertz, IEntityContainer entityContainer);
        void Stop();
    }

    public class PhysicsContainer : IPhysicsContainer
    {
        public Vector3 Gravity { private get; set; }
        public int Iterations { private get; set; } = 4;
        public bool Running { get; private set; }

        public void Start(int hertz, IEntityContainer entityContainer)
        {
            var timeStep = 1f / hertz;
            var ms = (int)(timeStep * 1000);
            Running = true;
            var task = new Task(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (Running)
                {
                    Simulate(timeStep, entityContainer);
                    stopwatch.Stop();
                    var dt = ms - (int)stopwatch.ElapsedMilliseconds;
                    if (dt > 0)
                        Thread.Sleep(dt);

                    stopwatch.Restart();
                }
            });
            task.Start();
        }

        public void Stop() => Running = false;

        public void Simulate(float timeStep, IEntityContainer entityContainer)
        {
            var dt = timeStep / Iterations;
            var entities = entityContainer.GetEntitiesWithComponent<PhysicsComponent>();

            // Update positions
            for (var i = 0; i < Iterations; i++)
            {
                var newPositions = new Dictionary<IEntity, Vector3>();
                var newOrientations = new Dictionary<IEntity, Vector3>();
                foreach (var entity in entities)
                {
                    var physicsComponent = entity.GetComponent<PhysicsComponent>();

                    var position = entity.GetComponent<PositionComponent>();
                    if (physicsComponent.Fixed && !physicsComponent.Kinematic)
                    {
                        newPositions.Add(entity, new Vector3(position.X, position.Y, position.Z));
                        newOrientations.Add(entity, new Vector3(position.Pitch, position.Roll, position.Yaw));
                    }
                    else
                    {
                        var (newPosition, newOrientation) = CalculatePosition(physicsComponent, position, dt);
                        newPositions.Add(entity, newPosition);
                        newOrientations.Add(entity, newOrientation);
                    }
                }
                foreach (var (entity, position) in newPositions)
                {
                    var physicsComponent = entity.GetComponent<PhysicsComponent>();

                    // Kinematic objects ignore collisions
                    if (physicsComponent.Kinematic)
                        ChangePosition(entity, position);

                    // Don't calculate collisions if fixed or kinematic
                    if (physicsComponent.Fixed || physicsComponent.Kinematic) continue;

                    var updatePosition = true;
                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var otherPosition = newPositions[other];
                        // var collision = DetermineCollision(entity, position, other, otherPosition);
                        var orientation = newOrientations[entity];
                        var otherOrientation = newOrientations[other];
                        var collision = DetermineCollision(entity, position, orientation, other, otherPosition, otherOrientation);

                        if (!collision) continue;

                        updatePosition = false;
                        var newPosition = InterpolateCollision(entity, other, position, otherPosition);
                        ChangePosition(entity, newPosition);
                    }
                    if (!updatePosition) continue;

                    ChangePosition(entity, position);
                }
            }

            // Clear force vectors
            foreach (var entity in entities)
            {
                var physicsComponent = entity.GetComponent<PhysicsComponent>();
                physicsComponent.Force = Vector3.Zero;
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
                    ePhysicsComponent.Velocity = new Vector3(0, ePhysicsComponent.Velocity.Y, ePhysicsComponent.Velocity.Z);
                }
                else
                {
                    var dh = (eSize.Height / 2 + oSize.Height / 2) * (position.Y > otherPosition.Y ? 1 : -1);
                    interpolatedPosition.Y = otherPosition.Y + dh;
                    ePhysicsComponent.Velocity = new Vector3(ePhysicsComponent.Velocity.X, 0, ePhysicsComponent.Velocity.Z);
                }
            }
            // TODO Simulate elastics collisions

            return interpolatedPosition;
        }

        private (Vector3 position, Vector3 orientation) CalculatePosition(PhysicsComponent physicsComponent, PositionComponent position, float timeStep)
        {
            var acceleration = DetermineAcceleration(physicsComponent);

            // Recalculate velocity
            var deltaV = acceleration * timeStep;
            physicsComponent.Velocity += deltaV;

            // Calculate delta p
            var (x, y, z) = physicsComponent.Velocity * timeStep + 0.5f * acceleration * timeStep * timeStep;

            // Calculate next position
            var newPosition = new Vector3(position.X + x, position.Y + y, position.Z + z);

            return (newPosition, new Vector3(position.Pitch, position.Roll, position.Yaw));
        }

        private Vector3 DetermineAcceleration(PhysicsComponent physicsComponent)
        {
            if (physicsComponent.Kinematic) return Vector3.Zero;

            if (physicsComponent.Force == default) return Gravity;

            return Gravity + (physicsComponent.Force / (physicsComponent.Mass == 0 ? 1 : physicsComponent.Mass));
        }

        private static bool DetermineCollision(IEntity a, Vector3 aNewPos, IEntity b, Vector3 bNewPos)
        {
            var (dx, dy) = CalculateDistance(aNewPos, a.GetComponent<SizeComponent>(), bNewPos, b.GetComponent<SizeComponent>());

            return dx < 0 && dy < 0;
        }

        private static bool DetermineCollision(IEntity a, Vector3 aNewPos, Vector3 aOr, IEntity b, Vector3 bNewPos, Vector3 bOr)
        {
            return Algorithms.GJK(aNewPos, aOr, a.GetComponent<SizeComponent>(), 
                bNewPos, bOr, b.GetComponent<SizeComponent>());
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
