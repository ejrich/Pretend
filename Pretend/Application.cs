using System;

namespace Pretend
{
    public interface IApplication
    {
        void Run();
    }

    public class Application : IApplication
    {
        public void Run()
        {
            Console.WriteLine("Hello World");
        }
    }
}
