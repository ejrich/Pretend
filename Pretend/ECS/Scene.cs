using System.Linq;
using OpenToolkit.Mathematics;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Physics;

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
        Vector3 Gravity { set; }
    }

    public class Scene : IScene
    {
        private readonly I2DRenderer _renderer;
        private readonly IEntityContainer _entityContainer;
        private readonly IPhysicsContainer _physicsContainer;

        public Scene(I2DRenderer renderer, IEntityContainer entityContainer, IPhysicsContainer physicsContainer)
        {
            _renderer = renderer;
            _entityContainer = entityContainer;
            _physicsContainer = physicsContainer;
        }

        public Vector3 Gravity { set => _physicsContainer.Gravity = value; }

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

            _physicsContainer.Simulate(timeStep, _entityContainer);
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
