using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pretend.Audio;
using Pretend.ECS;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Graphics.OpenGL;
using Pretend.Layers;
using Pretend.Physics;
using Pretend.Text;
using Pretend.UI;
using Pretend.Windows;

namespace Pretend
{
    public interface IFactory
    {
        T Create<T>();
        T Create<T>(Type type);
    }

    public class Factory : IFactory
    {
        private readonly IServiceCollection _services;
        private ServiceProvider _serviceProvider;

        public Factory() => _services = new ServiceCollection();

        public void RegisterServices<TApp, TSettings>() where TSettings : Settings, new()
        {
            // Logging
            _services.AddLogging(configure => configure.AddDebug());
            _services.AddTransient(typeof(ILog<>), typeof(Log<>));

            // Core services
            _services.AddTransient<IApplicationRunner, ApplicationRunner>();
            _services.AddSingleton<IEventDispatcher, EventDispatcher>();
            _services.AddSingleton<ILayerContainer, LayerContainer>();
            _services.AddSingleton<IFactory, Factory>(_ => this);
 
            // Settings
            _services.AddSingleton<SettingsManager<TSettings>>();
            _services.AddSingleton<ISettingsManager<TSettings>>(serviceProvider => serviceProvider.GetService<SettingsManager<TSettings>>());
            _services.AddSingleton<ISettingsManager<Settings>>(serviceProvider => serviceProvider.GetService<SettingsManager<TSettings>>());
            _services.AddSingleton<Settings>(serviceProvider => serviceProvider.GetService<SettingsManager<TSettings>>().Settings);

            // Windows
            _services.AddSingleton<IWindow, SDLWindow>();
            _services.AddSingleton<IInput, SDLInput>();

            // ECS
            _services.AddTransient<IScene, Scene>();
            _services.AddTransient<IEntityContainer, EntityContainer>();
            
            // Physics
            _services.AddTransient<IPhysicsContainer, PhysicsContainer>();

            // Core graphics
            _services.AddSingleton<IRenderer, Renderer>();
            _services.AddSingleton<I2DRenderer, Renderer2D>();
            _services.AddTransient<ICamera, OrthographicCamera>();

            // Graphics API
            _services.AddSingleton<IGraphicsContext, OpenGLContext>();
            _services.AddSingleton<IRenderContext, RenderContext>();
            _services.AddTransient<IVertexBuffer, VertexBuffer>();
            _services.AddTransient<IIndexBuffer, IndexBuffer>();
            _services.AddTransient<IVertexArray, VertexArray>();
            _services.AddTransient<IShader, Shader>();
            _services.AddTransient<ITexture2D, Texture2D>();

            // Audio API
            _services.AddSingleton<IAudioContext, AudioContext>();
            _services.AddTransient<IListener, Listener>();
            _services.AddTransient<ISoundBuffer, SoundBuffer>();
            _services.AddTransient<ISoundManager, SoundManager>();
            _services.AddTransient<ISource, Source>();

            // Text API
            _services.AddSingleton<ITextRenderer, TextRenderer>();
            _services.AddSingleton<IFont, Font>();

            // UI Library
            _services.AddTransient<IButton, Button>();

            // Application
            _services.AddTransient(typeof(IApplication), typeof(TApp));

            var assembly = typeof(TApp).Assembly;
            var classes = new HashSet<TypeInfo>();
            var interfaces = new HashSet<TypeInfo>();
            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsClass) classes.Add(type);
                else if (type.IsInterface) interfaces.Add(type);
            }

            foreach (var classType in classes)
            {
                if (IsSingleton(classType))
                    _services.AddSingleton(classType);
                else
                    _services.AddTransient(classType);
            }

            foreach (var interfaceType in interfaces)
            {
                var classType = classes.FirstOrDefault(_ => _.ImplementedInterfaces.Contains(interfaceType));
                if (classType == null) continue;
                if (IsSingleton(classType))
                    _services.AddSingleton(interfaceType, classType);
                else
                    _services.AddTransient(interfaceType, classType);
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

        public T Create<T>(Type type)
        {
            return (T)_serviceProvider.GetService(type);
        }

        private static bool IsSingleton(Type type)
        {
            return type.IsDefined(typeof(SingletonAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute
    {
    }
}
