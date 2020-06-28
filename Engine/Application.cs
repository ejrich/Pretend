using System;

namespace Engine
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
