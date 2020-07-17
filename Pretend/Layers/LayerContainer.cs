using System.Collections.Generic;
using Pretend.Events;

namespace Pretend.Layers
{
    public interface ILayerContainer
    {
        void PushLayer(ILayer layer);
        void RemoveLayer(ILayer layer);
        void Update(float timeStep);
    }

    public class LayerContainer : ILayerContainer
    {
        private readonly List<ILayer> _layers;

        public LayerContainer(IEventDispatcher eventDispatcher)
        {
            _layers = new List<ILayer>();

            eventDispatcher.Register(HandleEvent);
        }

        public void PushLayer(ILayer layer)
        {
            _layers.Add(layer);
            layer.Attach();
        }

        public void RemoveLayer(ILayer layer)
        {
            _layers.Remove(layer);
            layer.Detatch();
        }

        public void Update(float timeStep)
        {
            foreach (var layer in _layers)
            {
                layer.Update(timeStep);
            }
        }

        private void HandleEvent(IEvent evnt)
        {
            for (var i = _layers.Count - 1; i >= 0; --i)
            {
                if (evnt.Processed) return;
                _layers[i].HandleEvent(evnt);
            }
        }
    }
}
