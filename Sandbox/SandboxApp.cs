using Pretend;
using Pretend.Layers;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly ISandbox _sandbox;

        public SandboxApp(ILayerContainer layerContainer, ISandbox sandbox)
        {
            _layerContainer = layerContainer;
            _sandbox = sandbox;
        }

        public void Start()
        {
            switch (_sandbox.ActiveLayer)
            {
                case ActiveLayer.ExampleLayer:
                    _layerContainer.PushLayer<ExampleLayer>();
                    break;
                case ActiveLayer.Layer2D:
                    _layerContainer.PushLayer<Layer2D>();
                    break;
                case ActiveLayer.PhysicsLayer:
                    _layerContainer.PushLayer<PhysicsLayer>();
                    break;
                case ActiveLayer.TextLayer:
                    _layerContainer.PushLayer<TextLayer>();
                    break;
            }
            _layerContainer.PushLayer<SettingsLayer>();
        }
    }
}
