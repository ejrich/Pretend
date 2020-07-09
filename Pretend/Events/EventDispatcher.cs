using System;
using System.Collections.Generic;
using System.Linq;

namespace Pretend.Events
{
    public interface IEventDispatcher
    {
        void Register(Action<IEvent> callback);
        void Register<T>(Action<T> callback) where T : IEvent;
        void DispatchEvent(IEvent evnt);
        void DispatchEvent<T>(T evnt) where T : IEvent;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IEnumerable<Action<IEvent>> _eventHandlers;
        private readonly IDictionary<Type, IEnumerable<Action<IEvent>>> _typeEventHandlers;

        public EventDispatcher()
        {
            _eventHandlers = new List<Action<IEvent>>();
            _typeEventHandlers = new Dictionary<Type, IEnumerable<Action<IEvent>>>();
        }

        public void Register<T>(Action<T> callback) where T : IEvent
        {
            if (!_typeEventHandlers.TryGetValue(typeof(T), out var eventHandlers))
            {
                eventHandlers = new List<Action<IEvent>>();
                _typeEventHandlers.Add(typeof(T), eventHandlers);
            }

            eventHandlers.Append(evnt => callback((T) evnt));
        }

        public void Register(Action<IEvent> callback)
        {
            _eventHandlers.Append(callback);
        }

        public void DispatchEvent(IEvent evnt)
        {
            HandleEvent(evnt, _eventHandlers);
        }

        public void DispatchEvent<T>(T evnt) where T : IEvent
        {
            DispatchEvent((IEvent) evnt);

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
