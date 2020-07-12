using Pretend;

namespace Sandbox
{
    public class SandboxApp : IApplication
    {
        public WindowAttributes Attributes => new WindowAttributes { Title = "Sandbox" };
    }
}
