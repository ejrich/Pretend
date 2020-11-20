using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics;

namespace Pretend
{
    public interface IApplicationRunner
    {
        void Run(string title);
    }

    public class ApplicationRunner : IApplicationRunner
    {
        private readonly IApplication _application;
        private readonly IWindow _window;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILayerContainer _layerContainer;
        private readonly IRenderContext _renderContext;
        private readonly Settings _settings;

        private bool _running = true;

        public ApplicationRunner(IApplication application, IWindow window, IEventDispatcher eventDispatcher,
            ILayerContainer layerContainer, IRenderContext renderContext, Settings settings)
        {
            _application = application;
            _window = window;
            _eventDispatcher = eventDispatcher;
            _layerContainer = layerContainer;
            _renderContext = renderContext;
            _settings = settings;
        }

        public void Run(string title)
        {
            RegisterEvents();

            _window.Init(title, _settings);
            _application.Start();

            while (_running)
            {
                var timeStep = _window.GetTimestep();

                _renderContext.Clear();
                _layerContainer.Update(timeStep);
                _window.OnUpdate();
            }

            _layerContainer.RemoveAll();
            _application.Stop();
            _window.Close();
        }

        private void RegisterEvents()
        {
            _eventDispatcher.Register<WindowCloseEvent>(OnClose);
            _eventDispatcher.Register<WindowResizeEvent>(OnResize);
        }

        public void OnClose(WindowCloseEvent evnt)
        {
            _running = false;
        }

        public void OnResize(WindowResizeEvent evnt)
        {
            _renderContext.SetViewport(evnt.Width, evnt.Height);
        }
    }
}
