using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly Layer2D _layer2D;
        private readonly ILayer _exampleLayer;

        public SandboxApp(ILayerContainer layerContainer, ExampleLayer exampleLayer)
        {
            _layerContainer = layerContainer;
            // _layer2D = layer2D;
            _exampleLayer = exampleLayer;
        }

        public void Start()
        {
            // _layerContainer.PushLayer(_layer2D);
            _layerContainer.PushLayer(_exampleLayer);
        }
    }
    
    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Sandbox";
    }
}
