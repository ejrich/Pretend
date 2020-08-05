using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Pretend.Editor.ViewModels;
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
            Dispatcher.UIThread.Post(() =>
            {
                var viewModel = (MainWindowViewModel)DataContext;
                
                // TODO Have the framebuffer return a bitmap
                var bitmap = new System.Drawing.Bitmap(framebuffer.Width, framebuffer.Height);
                viewModel.Image = new Bitmap("Assets/picture.png");
            });
        }
    }
}
