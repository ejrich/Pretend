﻿using Pretend;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Entrypoint.Start<Application, GameSettings>("Possible Game");
        }
    }
}
