using System;
using System.Collections.Generic;
using System.Linq;
using Pretend.Events;
using Pretend.Graphics;

namespace Pretend.Layers
{
    public interface ILayerContainer
    {
        void PushLayer<T>() where T : ILayer;
        void PushLayer(ILayer layer);
        void RemoveLayer<T>() where T : ILayer;
        void RemoveLayer(ILayer layer);
        void SetLayerOrder(params Type[] layers);
        void Update(float timeStep);
        void RemoveAll();
    }

    public class LayerContainer : ILayerContainer
    {
        private readonly IFactory _factory;
        private readonly IRenderContext _renderContext;
        private List<ILayer> _layers = new List<ILayer>();
        private List<ILayer> _newLayers;

        public LayerContainer(IEventDispatcher eventDispatcher, IFactory factory, IRenderContext renderContext)
        {
            _factory = factory;
            _renderContext = renderContext;
            eventDispatcher.Register(HandleEvent);
        }

        public void PushLayer<T>() where T : ILayer
        {
            var layer = _factory.Create<T>();
            PushLayer(layer);
        }

        public void PushLayer(ILayer layer)
        {
            _layers.Add(layer);
            layer.Attach();
        }

        public void RemoveLayer<T>() where T : ILayer
        {
            var layer = _layers.FirstOrDefault(_ => _ is T);
            if (layer != null)
                RemoveLayer(layer);
        }

        public void RemoveLayer(ILayer layer)
        {
            _layers.Remove(layer);
            layer.Detach();
        }

        public void SetLayerOrder(params Type[] layers)
        {
            _newLayers = new List<ILayer>();
            foreach (var layerType in layers)
            {
                var layer = _layers.FirstOrDefault(_ => _.GetType() == layerType);
                if (layer == null)
                {
                    layer = _factory.Create<ILayer>(layerType);
                    layer.Attach();
                }
                _newLayers.Add(layer);
            }
        }

        public void Update(float timeStep)
        {
            foreach (var layer in _layers)
            {
                if (!layer.Paused)
                    layer.Update(timeStep);
                layer.Render();
                _renderContext.ClearDepth();
            }

            if (_newLayers == null)
                return;

            foreach (var layer in _layers.Where(l => !_newLayers.Contains(l)))
            {
                layer.Detach();
            }
            _layers = _newLayers;
            _newLayers = null;
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
