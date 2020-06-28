using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pretend
{
    public class Entrypoint
    {
        public static void Start<TApp>() where TApp : IApplication
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddConsole()
                .AddDebug());
            services.AddTransient(typeof(ILog<>), typeof(Log<>));

            services.AddTransient(typeof(IApplication), typeof(TApp));

            var provider = services.BuildServiceProvider();

            var application = provider.GetService<IApplication>();

            application.Run();
        }
    }
}
