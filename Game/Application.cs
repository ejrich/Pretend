using Pretend;
using Pretend.Layers;

namespace Game
{
    public class Application : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly GameLayer _gameLayer;
        private readonly SettingsLayer _settingsLayer;

        public Application(ILayerContainer layerContainer, GameLayer gameLayer, SettingsLayer settingsLayer)
        {
            _layerContainer = layerContainer;
            _gameLayer = gameLayer;
            _settingsLayer = settingsLayer;
        }

        public void Start()
        {
            _layerContainer.PushLayer(_gameLayer);
            _layerContainer.PushLayer(_settingsLayer);
        }
    }

    public class GameSettings : Settings
    {
        public bool Music { get; set; } = true;
        public bool SoundEffects { get; set; } = true;
    }
}
