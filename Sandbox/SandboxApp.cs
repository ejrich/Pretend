using Pretend;

namespace Sandbox
{
    public class SandboxApp : Application, IApplication
    {
        public SandboxApp(ILog<Application> log) : base(log) {}
    }
}
