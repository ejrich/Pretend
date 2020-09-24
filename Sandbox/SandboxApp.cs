using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly PhysicsLayer _physicsLayer;

        // private readonly ILayer _layer2D;
        // private readonly ILayer _exampleLayer;

        public SandboxApp(ILayerContainer layerContainer, PhysicsLayer physicsLayer)
                //Layer2D layer2D) 
                //ExampleLayer exampleLayer)
        {
            _layerContainer = layerContainer;
            _physicsLayer = physicsLayer;
            // _layer2D = layer2D;
            // _exampleLayer = exampleLayer;
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
