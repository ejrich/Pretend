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
                foreach (var entity in entities)
                {
                    var position = newPositions[entity];
                    var orientation = newOrientations[entity];
                    var physicsComponent = entity.GetComponent<PhysicsComponent>();

                    // Kinematic and non-solid object ignore collisions
                    if (physicsComponent.Kinematic || !physicsComponent.Solid)
                        ChangePosition(entity, position, orientation);

                    // Don't calculate collisions if fixed or kinematic
                    if (physicsComponent.Fixed || physicsComponent.Kinematic) continue;

                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var otherPosition = newPositions[other];
                        var otherOrientation = newOrientations[other];
                        var result = DetermineCollision(entity, position, orientation, other, otherPosition, otherOrientation);

                        if (!result.Collision) continue;

                        var (newPosition, newOrientation) = InterpolateCollision(entity, other, position, otherPosition, orientation, result);
                        position = newPosition;
                        orientation = newOrientation;
                    }

                    ChangePosition(entity, position, orientation);
                }
            }

            // Clear force vectors
            foreach (var entity in entities)
            {
                var physicsComponent = entity.GetComponent<PhysicsComponent>();
                physicsComponent.Force = Vector3.Zero;
            }
        }

        private (Vector3 position, Vector3 orientation) InterpolateCollision(IEntity entity, IEntity other,
            Vector3 position, Vector3 otherPosition, Vector3 orientation, GJKResult result)
        {
            var ePhysicsComponent = entity.GetComponent<PhysicsComponent>();

            // Don't try to move the position if the entity is fixed
            if (ePhysicsComponent.Fixed) return (position, orientation);

            var oPhysicsComponent = other.GetComponent<PhysicsComponent>();
            var interpolatedPosition = new Vector3(position);
            var interpolatedOrientation = new Vector3(orientation);

            // Simulate fixed collisions
            if (oPhysicsComponent.Fixed && oPhysicsComponent.Solid)
            {
                var epaResult = Algorithms.EPA(result);

                // Fix orientation based on the penetration vector
                var dOrientation = new Vector3(InterpolateOrientation(interpolatedOrientation.X, epaResult.Y, epaResult.Z),
                    InterpolateOrientation(interpolatedOrientation.Y, epaResult.Z, epaResult.X),
                    InterpolateOrientation(interpolatedOrientation.Z, epaResult.X, epaResult.Y));
                interpolatedOrientation += dOrientation;
                ePhysicsComponent.AngularVelocity = new Vector3(dOrientation.X != 0 ? 0 : ePhysicsComponent.AngularVelocity.X,
                    dOrientation.Y != 0 ? 0 : ePhysicsComponent.AngularVelocity.Y,
                    dOrientation.Z != 0 ? 0 : ePhysicsComponent.AngularVelocity.Z);

                // Correct the position based on the penetration vector
                var correctedResult = new Vector3(CorrectEPAResult(epaResult.X, epaResult.Yz, Gravity.Yz),
                    CorrectEPAResult(epaResult.Y, epaResult.Xz, Gravity.Xz),
                    CorrectEPAResult(epaResult.Z, epaResult.Xy, Gravity.Xy));
                interpolatedPosition -= correctedResult;
                ePhysicsComponent.Velocity = new Vector3(InterpolateVelocity(ePhysicsComponent.Velocity.X, correctedResult.X, Gravity.X),
                    InterpolateVelocity(ePhysicsComponent.Velocity.Y, correctedResult.Y, Gravity.Y),
                    InterpolateVelocity(ePhysicsComponent.Velocity.Z, correctedResult.Z, Gravity.Z));
            }
            // TODO Simulate elastics collisions

            return (interpolatedPosition, interpolatedOrientation);
        }

        private static float CorrectEPAResult(float a, Vector2 bc, Vector2 gbc)
        {
            return a == 0 || (bc[0] != 0 && gbc[0] != 0) || (bc[1] != 0 && gbc[1] != 0) ? 0 : a;
        }

        private static float InterpolateVelocity(float o, float p, float g)
        {
            if (Math.Sign(o) == Math.Sign(p)) return 0;
            return Math.Sign(o) + Math.Sign(g) == 0 ? 0 : o;
        }

        private static float InterpolateOrientation(float previous, float a, float b)
        {
            var theta = previous % 90;
            var newAngle = b == 0 ? 0 : (float) MathHelper.RadiansToDegrees(Math.Atan(a / b));
            return newAngle == 0 ? theta - newAngle > 45 ? 90 - theta - newAngle : newAngle - theta : 0;
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
            
            // Calculate dr
            var (p, r, yaw) = physicsComponent.AngularVelocity * timeStep;
            
            // Calculate next orientation
            var newOrientation = new Vector3(ChangeAngle(position.Pitch, p), ChangeAngle(position.Roll, r), ChangeAngle(position.Yaw, yaw));

            return (newPosition, newOrientation);
        }

        private static float ChangeAngle(float o, float d)
        {
            if (o < 0) o += 360;
            return (o - d) % 360;
        }

        private Vector3 DetermineAcceleration(PhysicsComponent physicsComponent)
        {
            if (physicsComponent.Kinematic) return Vector3.Zero;

            if (physicsComponent.Force == default) return Gravity;

            return Gravity + (physicsComponent.Force / (physicsComponent.Mass == 0 ? 1 : physicsComponent.Mass));
        }

        private static GJKResult DetermineCollision(IEntity a, Vector3 aNewPos, Vector3 aOr, IEntity b, Vector3 bNewPos, Vector3 bOr)
        {
            return Algorithms.GJK(aNewPos, aOr, a.GetComponent<SizeComponent>(), 
                bNewPos, bOr, b.GetComponent<SizeComponent>());
        }

        private static void ChangePosition(IEntity entity, Vector3 position, Vector3 orientation)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var (x, y, z) = position;
            positionComponent.X = x;
            positionComponent.Y = y;
            positionComponent.Z = z;
            positionComponent.Pitch = orientation.X;
            positionComponent.Roll = orientation.Y;
            positionComponent.Yaw = orientation.Z;
        }
    }
}
