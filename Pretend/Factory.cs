using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;
using Pretend.Layers;
using Pretend.Windows;

using System;
using System.Linq;

namespace Pretend
{
    public interface IFactory
    {
        T Create<T>();
    }

    public class Factory : IFactory
    {
        private readonly IServiceCollection _services;
        private ServiceProvider _serviceProvider;

        public Factory() => _services = new ServiceCollection();

        public void RegisterServices<TApp, TWA>()
        {
            // Logging
            _services.AddLogging(configure => configure.AddDebug());
            _services.AddTransient(typeof(ILog<>), typeof(Log<>));

            // Core services
            _services.AddTransient<IApplicationRunner, ApplicationRunner>();
            _services.AddSingleton<IEventDispatcher, EventDispatcher>();
            _services.AddSingleton<ILayerContainer, LayerContainer>();
            _services.AddSingleton<IFactory, Factory>(_ => this);

            // Windows
            _services.AddTransient<IWindow, SDLWindow>();
            _services.AddTransient<IInput, SDLInput>();

            // Core graphics
            _services.AddTransient<IRenderer, Renderer>();
            _services.AddTransient<ICamera, OrthographicCamera>();

            // Graphics API
            _services.AddTransient<IGraphicsContext, OpenGLContext>();
            _services.AddTransient<IRenderContext, RenderContext>();
            _services.AddTransient<IVertexBuffer, VertexBuffer>();
            _services.AddTransient<IIndexBuffer, IndexBuffer>();
            _services.AddTransient<IVertexArray, VertexArray>();
            _services.AddTransient<IShader, Shader>();
            _services.AddTransient<ITexture2D, Texture2D>();

            // Application
            _services.AddTransient(typeof(IApplication), typeof(TApp));
            _services.AddTransient(typeof(IWindowAttributesProvider), typeof(TWA));

            var assembly = typeof(TApp).Assembly;
            foreach (var type in assembly.DefinedTypes.Where(_ => !_.IsAbstract))
            {
                _services.AddTransient(type);
                Console.WriteLine($"{type.Name}: {type.IsClass}, {type.IsAbstract}, {type.IsInterface}");
            }
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
