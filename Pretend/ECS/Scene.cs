using System.Linq;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Mathematics;
using Pretend.Text;

namespace Pretend.ECS
{
    public interface IScene
    {
        void Init();
        IEntity CreateEntity();
        void DeleteEntity(IEntity entity);
        T AddComponent<T>(IEntity entity, T component) where T : IComponent;
        void HandleEvent(IEvent evnt);
        void Update(float timeStep);
        void Render();
        IEntityContainer EntityContainer { get; }
    }

    public class Scene : IScene
    {
        private readonly I2DRenderer _renderer;
        private readonly ITextRenderer _textRenderer;

        public Scene(I2DRenderer renderer, ITextRenderer textRenderer, IEntityContainer entityContainer)
        {
            _renderer = renderer;
            _textRenderer = textRenderer;
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

        public T AddComponent<T>(IEntity entity, T component) where T : IComponent
        {
            if (component is IScriptComponent scriptComponent)
            {
                scriptComponent.Attach();
                EntityContainer.AddComponent(entity, scriptComponent);
                return component;
            }
            EntityContainer.AddComponent(entity, component);
            return component;
        }

        public void HandleEvent(IEvent evnt)
        {
            foreach (var script in EntityContainer.GetComponents<IScriptComponent>())
            {
                if (evnt.Processed) return;
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
                            renderObject.Position = position.Position;
                            renderObject.Rotation = position.Rotation.ToQuaternian();
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

                var text = entity.GetComponent<TextComponent>();
                if (text != null)
                    RenderText(text, entity);
            }

            _renderer.End();
        }

        private void RenderText(TextComponent textComponent, IEntity entity)
        {
            if (string.IsNullOrWhiteSpace(textComponent.Text) || string.IsNullOrWhiteSpace(textComponent.Font) || textComponent.Size == 0)
                return;

            var textObject = new RenderableTextObject
            {
                Text = textComponent.Text,
                FontPath = textComponent.Font,
                Size = textComponent.Size,
                Alignment = textComponent.Alignment,
                Position = textComponent.RelativePosition,
                Orientation = textComponent.Orientation,
                Color = textComponent.Color
            };

            var position = entity.GetComponent<PositionComponent>();
            if (position != null)
                textObject.Position += position.Position;

            _textRenderer.RenderText(textObject);
        }
    }
}
