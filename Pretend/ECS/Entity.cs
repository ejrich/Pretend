using System;
using System.Collections.Generic;

namespace Pretend.ECS
{
    public interface IEntity
    {
        Guid Id { get; }
        IEnumerable<IComponent> Components { get; }
        void AddComponent<T>(T component) where T : IComponent;
        T GetComponent<T>() where T : IComponent;
    }

    public class Entity : IEntity
    {
        private readonly IDictionary<Type, IComponent> _componentMap = new Dictionary<Type, IComponent>();
        
        public Guid Id { get; } = Guid.NewGuid();
        public IEnumerable<IComponent> Components => _componentMap.Values;

        public void AddComponent<T>(T component) where T : IComponent
        {
            _componentMap.Add(typeof(T), component);
        }

        public T GetComponent<T>() where T : IComponent
        {
            _componentMap.TryGetValue(typeof(T), out var component);
            return (T) component;
        }
    }
}
