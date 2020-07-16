using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;
using Pretend.Layers;
using Pretend.Windows;

namespace Pretend
{
    public class Entrypoint
    {
        public static void Start<TApp>() where TApp : IApplication
        {
            var factory = new Factory();
            factory.RegisterServices<TApp>();
            factory.BuildContainer();

            var applicationRunner = factory.Create<IApplicationRunner>();

            applicationRunner.Run();
        }
    }
}
