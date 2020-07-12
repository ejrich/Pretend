using Pretend.Events;

namespace Pretend
{
    public interface IApplicationRunner
    {
        void Run();
    }

    public class ApplicationRunner : IApplicationRunner
    {
        private readonly IApplication _application;
        private readonly IWindow _window;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILayerContainer _layerContainer;
        private readonly ILog<ApplicationRunner> _log;

        private bool _running = true;

        public ApplicationRunner(IApplication application, IWindow window, IEventDispatcher eventDispatcher,
            ILayerContainer layerContainer, ILog<ApplicationRunner> log)
        {
            _application = application;
            _window = window;
            _eventDispatcher = eventDispatcher;
            _layerContainer = layerContainer;
            _log = log;
        }

        public void Run()
        {
            RegisterEvents();

            _log.Info("Hello World");

            _window.Init(_application.Attributes);
            _application.Start();

            while (_running)
            {
                _layerContainer.Update();
                _window.OnUpdate();
            }

            _application.Stop();
            _window.Close();
        }

        private void RegisterEvents()
        {
            _eventDispatcher.Register<WindowCloseEvent>(OnClose);
        }

        public void OnClose(WindowCloseEvent evnt)
        {
            _running = false;
        }
    }
}
