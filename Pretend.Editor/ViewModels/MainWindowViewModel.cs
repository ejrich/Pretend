using System;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Pretend.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _name;

        public string Greeting => "Hello World!";

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public Action<string> Run => RunImpl;

        private void RunImpl(string value)
        {
            Console.WriteLine(value);
        }

        public IBitmap Image => new Bitmap("Assets/picture.png");
    }
}
