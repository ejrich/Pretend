using System.Linq;
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
        IEntityContainer EntityContainer { get; }
    }

    public class Scene : IScene
    {
        private readonly I2DRenderer _renderer;

        public Scene(I2DRenderer renderer, IEntityContainer entityContainer)
        {
            _renderer = renderer;
            EntityContainer = entityContainer;
        }

        public IEntityContainer EntityContainer { get; }

        public void Init()
        {
            _renderer.Init();
        }

        public IEntity CreateEntity()
        {
            return EntityContainer.CreateEntity();
        }

        public void DeleteEntity(IEntity entity)
        {
            if (entity == null) return;

            entity.GetComponent<IScriptComponent>()?.Detach();
            EntityContainer.DeleteEntity(entity);
        }

        public void AddComponent<T>(IEntity entity, T component) where T : IComponent
        {
            if (component is IScriptComponent scriptComponent)
            {
                scriptComponent.Attach();
                EntityContainer.AddComponent(entity, scriptComponent);
                return;
            }
            EntityContainer.AddComponent(entity, component);
        }

        public void HandleEvent(IEvent evnt)
        {
            foreach (var script in EntityContainer.GetComponents<IScriptComponent>())
            {
                script.HandleEvent(evnt);
            }
        }

        public void Update(float timeStep)
        {
            foreach (var script in EntityContainer.GetComponents<IScriptComponent>())
            {
                script.Update(timeStep);
            }
        }

        public void Render()
        {
            var cameraComponent = EntityContainer.GetComponents<CameraComponent>()
                .FirstOrDefault(camera => camera.Active);

            _renderer.Begin(cameraComponent?.Camera);

            foreach (var entity in EntityContainer.Entities)
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
