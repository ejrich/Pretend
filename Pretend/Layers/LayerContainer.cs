using System.Collections.Generic;
using Pretend.Events;

namespace Pretend.Layers
{
    public interface ILayerContainer
    {
        void PushLayer(ILayer layer);
        void RemoveLayer(ILayer layer);
        void Update(float timeStep);
        void RemoveAll();
    }

    public class LayerContainer : ILayerContainer
    {
        private readonly List<ILayer> _layers = new List<ILayer>();

        public LayerContainer(IEventDispatcher eventDispatcher)
        {
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
            layer.Detach();
        }

        public void Update(float timeStep)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Paused)
                    layer.Update(timeStep);
                layer.Render();
            }
        }

        public void RemoveAll()
        {
            foreach (var layer in _layers)
                layer.Detach();
            _layers.Clear();
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
