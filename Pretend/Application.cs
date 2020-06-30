using System.Threading.Tasks;
using OpenToolkit;

namespace Pretend
{
    public interface IApplication
    {
        void Run();
    }

    public class Application : IApplication
    {
        private readonly ILog<Application> _log;

        public Application(ILog<Application> log)
        {
            _log = log;
        }

        public void Run()
        {
            _log.Info("Hello World");
            var window = new Window();

            window.Show();
        }
    }
}
