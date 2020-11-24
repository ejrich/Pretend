using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;

        public SandboxApp(ILayerContainer layerContainer)
        {
            _layerContainer = layerContainer;
        }

        public void Start()
        {
            _layerContainer.PushLayer<TextLayer>();
            _layerContainer.PushLayer<SettingsLayer>();
        }
    }
}
