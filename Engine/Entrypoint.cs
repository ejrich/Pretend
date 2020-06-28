using System;
using Microsoft.Extensions.DependencyInjection;

namespace Engine
{
    public class Entrypoint
    {
        public static void Start<TApp>() where TApp : IApplication
        {
            var services = new ServiceCollection();

            services.AddTransient(typeof(IApplication), typeof(TApp));

            var provider = services.BuildServiceProvider();

            var application = provider.GetService<IApplication>();

            application.Run();
        }
    }
}
