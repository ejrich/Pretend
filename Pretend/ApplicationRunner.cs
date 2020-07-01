namespace Pretend
{
    public interface IApplicationRunner
    {
        void Run();
    }

    public class ApplicationRunner : IApplicationRunner
    {
        private readonly IApplication _application;
        private readonly ILog<ApplicationRunner> _log;

        private bool _running = true;

        public ApplicationRunner(IApplication application, ILog<ApplicationRunner> log)
        {
            _application = application;
            _log = log;
        }

        public void Run()
        {
            _log.Info("Hello World");
            var window = new Window();

            _application.Start();

            while (_running)
            {
                window.OnUpdate();
                // _running = false;
            }

            _application.Stop();
        }
    }
}
