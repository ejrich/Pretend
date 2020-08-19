using System.Collections.Generic;
using Pretend.Graphics;

namespace Pretend.ECS
{
    public interface IScene
    {
        void Init();
        Entity CreateEntity();
        void Update();
        void Render();
    }

    public class Scene : IScene
    {
        private readonly I2DRenderer _renderer;

        private readonly List<Entity> _entities = new List<Entity>();

        public Scene(I2DRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Init()
        {
            _renderer.Init();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity();
            _entities.Add(entity);
            return entity;
        }

        public void Update()
        {
            // TODO Update components
        }

        public void Render()
        {
            foreach (var entity in _entities)
            {
                var renderObject = new Renderable2DObject();
                var position = entity.GetComponent<PositionComponent>();
                if (position != null)
                {
                    renderObject.X = position.X;
                    renderObject.Y = position.Y;
                    renderObject.Z = position.Z;
                    renderObject.Rotation = position.Rotation;
                }

                var size = entity.GetComponent<SizeComponent>();
                if (size != null)
                {
                    renderObject.Width = size.Width;
                    renderObject.Height = size.Height;
                }

                var color = entity.GetComponent<ColorComponent>();
                if (color != null)
                    renderObject.Color = color.Color;

                var texture = entity.GetComponent<TextureComponent>();
                if (texture != null)
                    renderObject.Texture = texture.Texture;

                _renderer.Submit(renderObject);
            }
        }
    }
}
