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
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddDebug());
            services.AddTransient(typeof(ILog<>), typeof(Log<>));

            services.AddTransient<IApplicationRunner, ApplicationRunner>();
            services.AddTransient<IWindow, SDLWindow>();
            services.AddTransient<IInput, SDLInput>();
            services.AddTransient<IGraphicsContext, OpenGLContext>();
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.AddSingleton<ILayerContainer, LayerContainer>();
            services.AddTransient(typeof(IApplication), typeof(TApp));

            var provider = services.BuildServiceProvider();

            var applicationRunner = provider.GetService<IApplicationRunner>();

            applicationRunner.Run();
        }
    }
}
