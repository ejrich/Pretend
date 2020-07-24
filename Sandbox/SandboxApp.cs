using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly Layer2D _layer2D;
        // private readonly ILayer _exampleLayer;

        public SandboxApp(ILayerContainer layerContainer, Layer2D layer2D) //ExampleLayer exampleLayer)
        {
            _layerContainer = layerContainer;
            _layer2D = layer2D;
            // _exampleLayer = exampleLayer;
        }

        public void Start()
        {
            _layerContainer.PushLayer(_layer2D);
        }
    }
    
    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Sandbox";
    }
}
