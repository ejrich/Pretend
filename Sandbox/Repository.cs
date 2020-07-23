using System;
using Pretend;

namespace Sandbox
{
    public interface IRepository
    {
        void DoSomething();
    }

    [Singleton]
    public class Repository : IRepository
    {
        public void DoSomething()
        {
            Console.WriteLine("Hello world");
        }
    }
}