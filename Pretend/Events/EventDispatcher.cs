using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.Events
{
    public interface IEventDispatcher
    {
        void Register(Action<IEvent> callback);
        void Register<T>(Action<T> callback) where T : IEvent;
        void DispatchEvent<T>(T evnt) where T : IEvent;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IList<Action<IEvent>> _eventHandlers;
        private readonly IDictionary<Type, IList<Action<IEvent>>> _typeEventHandlers;

        public EventDispatcher()
        {
            _eventHandlers = new List<Action<IEvent>>();
            _typeEventHandlers = new Dictionary<Type, IList<Action<IEvent>>>();
        }

        public void Register<T>(Action<T> callback) where T : IEvent
        {
            if (!_typeEventHandlers.TryGetValue(typeof(T), out var eventHandlers))
            {
                eventHandlers = new List<Action<IEvent>>();
                _typeEventHandlers.Add(typeof(T), eventHandlers);
            }

            eventHandlers.Add(evnt => callback((T) evnt));
        }

        public void Register(Action<IEvent> callback)
        {
            _eventHandlers.Add(callback);
        }

        public void DispatchEvent<T>(T evnt) where T : IEvent
        {
            HandleEvent(evnt, _eventHandlers);

            var type = evnt.GetType();

            if (_typeEventHandlers.TryGetValue(type, out var eventHandlers))
                HandleEvent(evnt, eventHandlers);
        }

        private void HandleEvent(IEvent evnt, IEnumerable<Action<IEvent>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                eventHandler(evnt);
            }
        }
    }
}
