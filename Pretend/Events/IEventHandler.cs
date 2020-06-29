using System;

namespace Pretend.Events
{
    public interface IEventHandler
    {
        void Handle(IEvent evnt);
    }

    public abstract class IEventHandler<T> : IEventHandler where T : IEvent
    {
        public abstract void Handle(T evnt);
        public void Handle(IEvent evnt) { Handle((T) evnt); }
    }
}
