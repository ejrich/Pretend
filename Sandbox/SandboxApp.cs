using Pretend;
using Pretend.Layers;
using Pretend.Graphics;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly IRenderer _renderer;

        public SandboxApp(ILayerContainer layerContainer, IRenderer renderer)
        {
            _layerContainer = layerContainer;
            _renderer = renderer;
        }

        public WindowAttributes Attributes => new WindowAttributes { Title = "Sandbox" };

        public void Start()
        {
            var layer = new ExampleLayer(_renderer);

            _layerContainer.PushLayer(layer);
        }
    }
}
