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
        private readonly ILog<ApplicationRunner> _log;

        public ApplicationRunner(IApplication application, IWindow window, ILog<ApplicationRunner> log)
        {
            _application = application;
            _window = window;
            _log = log;
        }

        public void Run()
        {
            _log.Info("Hello World");

            _window.Init();
            _application.Start();

            while (!_window.IsClosing)
            {
                _window.OnUpdate();
            }

            _application.Stop();
            _window.Close();
        }
    }
}
