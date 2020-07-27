using Pretend;
using Pretend.Layers;

namespace Game
{
    public class Application : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly GameLayer _gameLayer;

        public Application(ILayerContainer layerContainer, GameLayer gameLayer)
        {
            _layerContainer = layerContainer;
            _gameLayer = gameLayer;
        }

        public void Start()
        {
            _layerContainer.PushLayer(_gameLayer);
        }
    }

    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Possible Game";
    }
}
