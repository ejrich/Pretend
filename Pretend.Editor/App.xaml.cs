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
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }
            var app = new Thread(Entrypoint.Start<EditorApp, WindowAttributes>);
            app.Start();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
