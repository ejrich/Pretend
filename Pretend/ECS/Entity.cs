using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.ECS
{
    public class Entity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public List<IComponent> Components { get; } = new List<IComponent>();

        public T GetComponent<T>()
        {
            var component = Components.FirstOrDefault(_ => _ is T);
            return (T) component;
        }
    }
}
