using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pretend.ECS
{
    public interface IEntityContainer
    {
        List<IEntity> Entities { get; }
        List<T> GetComponents<T>() where T : IComponent;
        List<IEntity> GetEntitiesWithComponent<T>() where T : IComponent;
        void AddComponent<T>(IEntity entity, T component) where T : IComponent;
        IEntity CreateEntity();
        void DeleteEntity(IEntity entity);
    }

    public class EntityContainer : IEntityContainer
    {
        private readonly IDictionary<Guid, IEntity> _entityDictionary = new Dictionary<Guid, IEntity>();
        private readonly IDictionary<Type, List<IComponent>> _components = new Dictionary<Type, List<IComponent>>();
        private readonly IDictionary<Type, List<IEntity>> _componentEntityDictionary = new Dictionary<Type, List<IEntity>>();

        public List<IEntity> Entities => _entityDictionary.Values.ToList();

        public List<T> GetComponents<T>() where T : IComponent
        {
            return _components.TryGetValue(typeof(T), out var components) ?
                components.Select(_ => (T) _).ToList() : new List<T>();
        }

        public List<IEntity> GetEntitiesWithComponent<T>() where T : IComponent
        {
            return _componentEntityDictionary.TryGetValue(typeof(T), out var entities) ?
                entities : new List<IEntity>();
        }

        public void AddComponent<T>(IEntity entity, T component) where T : IComponent
        {
            entity.AddComponent(component);

            if (!_components.TryGetValue(typeof(T), out var components))
                _components[typeof(T)] = components = new List<IComponent>();
            components.Add(component);
            
            if (!_componentEntityDictionary.TryGetValue(typeof(T), out var entities))
                _componentEntityDictionary[typeof(T)] = entities = new List<IEntity>();
            entities.Add(entity);
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

                if (_componentEntityDictionary.TryGetValue(type, out var entities))
                    entities.Remove(entity);
            }
        }
    }
}
