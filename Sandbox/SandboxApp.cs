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

        public WindowAttributes Attributes => new WindowAttributes { Title = "Sandbox" };

        public void Start()
        {
            var layer = new ExampleLayer();

            _layerContainer.PushLayer(layer);
        }
    }
}
