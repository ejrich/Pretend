using Pretend.Events;

namespace Pretend.Layers
{
    public interface ILayer
    {
        void Attach() {}
        void Detatch() {}
        void Update();
        void HandleEvent(IEvent evnt);
    }
}
