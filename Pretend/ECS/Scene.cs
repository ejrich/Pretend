﻿using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.Events;
using Pretend.Graphics;

namespace Pretend.ECS
{
    public interface IScene
    {
        void Init();
        IEntity CreateEntity();
        void DeleteEntity(IEntity entity);
        void AddComponent<T>(IEntity entity, T component) where T : IComponent;
        void HandleEvent(IEvent evnt);
        void Update(float timeStep);
        void Render();
    }

    public class Scene : IScene
    {
        private readonly I2DRenderer _renderer;
        private readonly IEntityContainer _entityContainer;

        public Scene(I2DRenderer renderer, IEntityContainer entityContainer)
        {
            _renderer = renderer;
            _entityContainer = entityContainer;
        }

        public void Init()
        {
            _renderer.Init();
        }

        public IEntity CreateEntity()
        {
            return _entityContainer.CreateEntity();
        }

        public void DeleteEntity(IEntity entity)
        {
            if (entity == null) return;

            entity.GetComponent<IScriptComponent>()?.Detach();
            _entityContainer.DeleteEntity(entity);
        }

        public void AddComponent<T>(IEntity entity, T component) where T : IComponent
        {
            if (component is IScriptComponent scriptComponent)
            {
                scriptComponent.Attach();
                _entityContainer.AddComponent(entity, scriptComponent);
                return;
            }
            _entityContainer.AddComponent(entity, component);
        }

        public void HandleEvent(IEvent evnt)
        {
            foreach (var script in _entityContainer.GetComponents<IScriptComponent>())
            {
                script.HandleEvent(evnt);
            }
        }

        public void Update(float timeStep)
        {
            foreach (var script in _entityContainer.GetComponents<IScriptComponent>())
            {
                script.Update(timeStep);
            }

            SimulateWorld(timeStep);
        }

        private void SimulateWorld(float timeStep)
        {
            var entities = _entityContainer.GetEntitiesWithComponent<PhysicsComponent>();
            foreach (var entity in entities)
            {
                var physicsComponent = entity.GetComponent<PhysicsComponent>();
                if (physicsComponent.Fixed) continue;

                var positionComponent = entity.GetComponent<PositionComponent>();
                var gravity = new Vector3(0, -800, 0);

                CalculatePosition(gravity, physicsComponent, positionComponent, timeStep);
            }
        }

        private void CalculatePosition(Vector3 gravity, PhysicsComponent physicsComponent, PositionComponent position, float timeStep)
        {
            // Calculate delta p
            var (x, y, z) = physicsComponent.Velocity * timeStep + 0.5f * gravity * timeStep * timeStep;

            // Calculate next position
            position.X += x;
            position.Y += y;
            position.Z += z;

            // Recalculate velocity
            var deltaV = gravity * timeStep;
            physicsComponent.Velocity += deltaV;
        }

        public void Render()
        {
            var cameraComponent = _entityContainer.GetComponents<CameraComponent>()
                .FirstOrDefault(camera => camera.Active);

            _renderer.Begin(cameraComponent?.Camera);

            foreach (var entity in _entityContainer.Entities)
            {
                var renderObject = new Renderable2DObject();
                foreach (var component in entity.Components)
                {
                    switch (component)
                    {
                        case PositionComponent position:
                            renderObject.X = position.X;
                            renderObject.Y = position.Y;
                            renderObject.Z = position.Z;
                            renderObject.Rotation = position.Rotation;
                            break;
                        case SizeComponent size:
                            renderObject.Width = size.Width;
                            renderObject.Height = size.Height;
                            break;
                        case ColorComponent color:
                            renderObject.Color = color.Color;
                            break;
                        case TextureComponent texture:
                            renderObject.Texture = texture.Texture;
                            break;
                    }
                }
                _renderer.Submit(renderObject);
            }

            _renderer.End();
        }
    }
}
