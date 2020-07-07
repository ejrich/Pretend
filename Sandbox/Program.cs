using System;
using Pretend;
using SDL2;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Entrypoint.Start<SandboxApp>();
        }
    }
}
