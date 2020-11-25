using System;
using System.Numerics;

namespace Pretend
{
    [Flags]
    public enum WindowMode
    {
        Windowed = 0,
        Fullscreen = 1,
        Borderless = 2
    }

    public interface IWindow
    {
        void Init(string title, Settings settings);
        float GetTimestep();
        void OnUpdate();
        void Close();
        Vector<int> Resolution { get; set; }
        ushort MaxFps { set; }
        WindowMode WindowMode { set; }
        bool MouseGrab { set; }
    }
}
