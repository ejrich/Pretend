using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.Events
{
    public interface IEventDispatcher
    {
        void DispatchEvent(IEvent @event);
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IDictionary<Type, IEventHandler> _eventHandlers;

        public EventDispatcher(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers.ToDictionary(GetEventType);
        }

        public void DispatchEvent(IEvent evnt)
        {
            var type = evnt.GetType();

            if (_eventHandlers.TryGetValue(type, out var handler))
                handler.Handle(evnt);
        }

        private static Type GetEventType(IEventHandler eventHandler)
        {
            return eventHandler.GetType()
                .BaseType
                .GetGenericArguments()
                .First();
        }
    }
}
