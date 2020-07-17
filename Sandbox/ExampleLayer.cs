using Pretend.Events;
using Pretend.Layers;

namespace Sandbox
{
    public class ExampleLayer : ILayer
    {
        public void Update(float timeStep)
        {
            // Do something
        }

        public void HandleEvent(IEvent evnt)
        {
            // Handle an event
        }
    }
}
