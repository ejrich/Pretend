using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Pretend.Editor.ViewModels;
using Pretend.Editor.Views;

namespace Pretend.Editor
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var factory = new Factory();
            factory.RegisterServices<EditorApp, WindowAttributes>();
            factory.BuildContainer();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = factory.Create<MainWindow>();
                mainWindow.DataContext = new MainWindowViewModel();

                desktop.MainWindow = mainWindow;
            }

            var applicationRunner = factory.Create<IApplicationRunner>();

            var app = new Thread(applicationRunner.Run);
            app.Start();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
