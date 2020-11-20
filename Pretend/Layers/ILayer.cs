using Pretend.Events;

namespace Pretend.Layers
{
    public interface ILayer
    {
        void Attach() {}
        void Detach() {}
        void Pause() {}
        void Resume() {}
        bool Paused { get; }
        void Update(float timeStep);
        void Render();
        void HandleEvent(IEvent evnt);
    }
}
