﻿using System;
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
            var ms = (int)(timeStep * 1000) * 1;
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

                    // Kinematic objects ignore collisions
                    if (physicsComponent.Kinematic)
                        ChangePosition(entity, position, orientation);

                    // Don't calculate collisions if fixed or kinematic
                    if (physicsComponent.Fixed || physicsComponent.Kinematic) continue;

                    var updatePosition = true;
                    foreach (var other in entities.Where(_ => _ != entity))
                    {
                        var otherPosition = newPositions[other];
                        var otherOrientation = newOrientations[other];
                        var result = DetermineCollision(entity, position, orientation, other, otherPosition, otherOrientation);

                        if (!result.Collision) continue;

                        updatePosition = false;
                        var (newPosition, newOrientation) = InterpolateCollision(entity, other, position, otherPosition, orientation, result);
                        ChangePosition(entity, newPosition, newOrientation);
                    }
                    if (!updatePosition) continue;

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
            if (oPhysicsComponent.Fixed)
            {
                var epaResult = Algorithms.EPA(result);
                interpolatedPosition -= epaResult;
                ePhysicsComponent.Velocity = new Vector3(InterpolatedVelocity(ePhysicsComponent.Velocity.X, epaResult.X, Gravity.X),
                    InterpolatedVelocity(ePhysicsComponent.Velocity.Y, epaResult.Y, Gravity.Y),
                    InterpolatedVelocity(ePhysicsComponent.Velocity.Z, epaResult.Z, Gravity.Z));

                // TODO Make this calculation based on acceleration
                var yaw = interpolatedOrientation.Z % 90;
                var newAngle = epaResult.Y == 0 ? 0 : (float) MathHelper.RadiansToDegrees(Math.Atan(epaResult.X / epaResult.Y));
                interpolatedOrientation.Z += yaw - newAngle > 45 ? 90 - yaw - newAngle : newAngle - yaw;
            }
            // TODO Simulate elastics collisions

            return (interpolatedPosition, interpolatedOrientation);
        }

        private static float InterpolatedVelocity(float o, float p, float g)
        {
            if (Math.Sign(o) == Math.Sign(p)) return 0;
            return Math.Sign(o) + Math.Sign(g) == 0 ? 0 : o;
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
            Console.WriteLine(orientation.Z);
            positionComponent.Yaw = orientation.Z;
        }
    }
}
