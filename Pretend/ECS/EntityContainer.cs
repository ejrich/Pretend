using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.ECS
{
    public interface IEntityContainer
    {
        List<IEntity> Entities { get; }
        List<T> GetComponents<T>() where T : IComponent;
        void AddComponent<T>(IEntity entity, T component) where T : IComponent;
        IEntity CreateEntity();
        void DeleteEntity(IEntity entity);
    }

    public class EntityContainer : IEntityContainer
    {
        private readonly IDictionary<Guid, IEntity> _entityDictionary = new Dictionary<Guid, IEntity>();
        private readonly IDictionary<Type, List<IComponent>> _components = new Dictionary<Type, List<IComponent>>();

        public List<IEntity> Entities => _entityDictionary.Values.ToList();

        public List<T> GetComponents<T>() where T : IComponent
        {
            return !_components.TryGetValue(typeof(T), out var components) ?
                new List<T>() : components.Select(_ => (T) _).ToList();
        }

        public void AddComponent<T>(IEntity entity, T component) where T : IComponent
        {
            entity.AddComponent(component);

            if (!_components.TryGetValue(typeof(T), out var components))
                _components[typeof(T)] = components = new List<IComponent>();

            components.Add(component);
        }

        public IEntity CreateEntity()
        {
            var entity = new Entity();
            _entityDictionary[entity.Id] = entity;

            return entity;
        }

        public void DeleteEntity(IEntity entity)
        {
            _entityDictionary.Remove(entity.Id);
            foreach (var component in entity.Components)
            {
                var type = component is IScriptComponent ? typeof(IScriptComponent) : component.GetType();
                if (_components.TryGetValue(type, out var components))
                    components.Remove(component);
            }
        }
    }
}
