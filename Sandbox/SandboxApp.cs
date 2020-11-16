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
        private readonly TextLayer _textLayer;
        private readonly SettingsLayer _settingsLayer;

        public SandboxApp(ILayerContainer layerContainer, Layer2D layer2D, ExampleLayer exampleLayer,
            PhysicsLayer physicsLayer, TextLayer textLayer, SettingsLayer settingsLayer)
        {
            _layerContainer = layerContainer;
            _exampleLayer = exampleLayer;
            _layer2D = layer2D;
            _physicsLayer = physicsLayer;
            _textLayer = textLayer;
            _settingsLayer = settingsLayer;
        }

        public void Start()
        {
            // _layerContainer.PushLayer(_exampleLayer);
            // _layerContainer.PushLayer(_layer2D);
            // _layerContainer.PushLayer(_physicsLayer);
            _layerContainer.PushLayer(_textLayer);
            _layerContainer.PushLayer(_settingsLayer);
        }
    }
}
