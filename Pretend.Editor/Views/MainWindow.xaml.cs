using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Pretend.Graphics;

namespace Pretend.Editor.Views
{
    public class MainWindow : Window
    {
        private readonly IFramebufferCapture _framebufferCapture;

        public MainWindow() : this(null) { }

        public MainWindow(IFramebufferCapture framebufferCapture)
        {
            _framebufferCapture = framebufferCapture;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _framebufferCapture.Callback = SetFrame;
            AvaloniaXamlLoader.Load(this);
        }

        private void SetFrame(IFramebuffer framebuffer)
        {
            
        }
    }
}
