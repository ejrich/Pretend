using System.Linq;
using Pretend.Events;
using Pretend.Graphics;

namespace Pretend.ECS
{
    public interface IScene
    {
        void Init();
        Entity CreateEntity();
        void AddComponent<T>(Entity entity, T component) where T : IComponent;
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

        public Entity CreateEntity()
        {
            return _entityContainer.CreateEntity();
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            if (component is IScriptComponent scriptComponent)
            {
                // TODO Run script lifecycle methods
                // AddComponent(entity, scriptComponent);
            }
            _entityContainer.AddComponent(entity, component);
        }

        private void AddComponent(Entity entity, IScriptComponent component)
        {
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
