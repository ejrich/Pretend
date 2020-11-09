using System;

namespace Pretend
{
    [Flags]
    public enum WindowMode
    {
        Windowed = 1,
        Fullscreen = 2,
        Borderless = 4
    }

    public interface IWindow
    {
        void Init(string title, ISettingsManager settings);
        float GetTimestep();
        void OnUpdate();
        void Close();
    }
}
