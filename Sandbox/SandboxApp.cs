using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly Layer2D _layer2D;
        private readonly ExampleLayer _exampleLayer;
        private readonly PhysicsLayer _physicsLayer;

        public SandboxApp(ILayerContainer layerContainer, Layer2D layer2D, ExampleLayer exampleLayer,
            PhysicsLayer physicsLayer)
        {
            _layerContainer = layerContainer;
            _exampleLayer = exampleLayer;
            _layer2D = layer2D;
            _physicsLayer = physicsLayer;
        }

        public void Start()
        {
            _layerContainer.PushLayer(_physicsLayer);
            // _layerContainer.PushLayer(_layer2D);
            // _layerContainer.PushLayer(_exampleLayer);
        }
    }
    
    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Sandbox";
    }
}
