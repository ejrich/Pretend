using Pretend;
using Pretend.Layers;

namespace Game
{
    public class Application : IApplication
    {
        private readonly ILayerContainer _layerContainer;

        public Application(ILayerContainer layerContainer)
        {
            _layerContainer = layerContainer;
        }

        public void Start()
        {
            _layerContainer.PushLayer<GameLayer>();
        }
    }

    public class GameSettings : Settings
    {
        public bool Music { get; set; } = true;
        public bool SoundEffects { get; set; } = true;
    }
}
