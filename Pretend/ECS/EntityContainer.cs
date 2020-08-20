using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.ECS
{
    public interface IEntityContainer
    {
        List<Entity> Entities { get; }
        List<T> GetComponents<T>() where T : IComponent;
        void AddComponent<T>(Entity entity, T component) where T : IComponent;
        Entity CreateEntity();
    }

    public class EntityContainer : IEntityContainer
    {
        private readonly IDictionary<Guid, Entity> _entityDictionary = new Dictionary<Guid, Entity>();
        private readonly IDictionary<Type, List<IComponent>> _components = new Dictionary<Type, List<IComponent>>();

        public List<Entity> Entities { get; } = new List<Entity>();

        public List<T> GetComponents<T>() where T : IComponent
        {
            return !_components.TryGetValue(typeof(T), out var components) ?
                new List<T>() : components.Select(_ => (T) _).ToList();
        }

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            entity.Components.Add(component);

            if (!_components.TryGetValue(typeof(T), out var components))
                _components[typeof(T)] = components = new List<IComponent>();

            components.Add(component);
        }

        public Entity CreateEntity()
        {
            var entity = new Entity();
            Entities.Add(entity);
            _entityDictionary[entity.Id] = entity;

            return entity;
        }
    }
}
