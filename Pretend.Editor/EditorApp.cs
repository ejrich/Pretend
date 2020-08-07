using Pretend.Layers;

namespace Pretend.Editor
{
    public class EditorApp : IApplication
    {
        private readonly ILayerContainer _layerContainer;
        private readonly ILayer _editorLayer;

        public EditorApp(ILayerContainer layerContainer, EditorLayer editorLayer)
        {
            _layerContainer = layerContainer;
            _editorLayer = editorLayer;
        }

        public void Start()
        {
            _layerContainer.PushLayer(_editorLayer);
        }
    }

    public class WindowAttributes : IWindowAttributesProvider
    {
        public string Title => "Editor";
        public int Width => 1920;
        public int Height => 1080;
    }
}
