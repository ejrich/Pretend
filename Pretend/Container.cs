using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;
using Pretend.Layers;
using Pretend.Windows;

namespace Pretend
{
    public interface IContainer
    {
        T Create<T>();
    }

    public class Container : IContainer
    {
        private readonly IServiceCollection _services;
        private ServiceProvider _serviceProvider;

        public Container() => _services = new ServiceCollection();

        public void RegisterServices<TApp>()
        {
            _services.AddLogging(configure => configure.AddConsole()
                .AddDebug());
            _services.AddTransient(typeof(ILog<>), typeof(Log<>));

            _services.AddTransient<IApplicationRunner, ApplicationRunner>();
            _services.AddTransient<IWindow, SDLWindow>();
            _services.AddTransient<IInput, SDLInput>();
            _services.AddTransient<IGraphicsContext, OpenGLContext>();
            _services.AddSingleton<IEventDispatcher, EventDispatcher>();
            _services.AddSingleton<ILayerContainer, LayerContainer>();
            _services.AddSingleton<IContainer, Container>(_ => this);
            _services.AddTransient(typeof(IApplication), typeof(TApp));
        }

        public void BuildContainer()
        {
            _serviceProvider = _services.BuildServiceProvider();
        }

        public T Create<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
