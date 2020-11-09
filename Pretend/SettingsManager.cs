using OpenTK.Mathematics;
using Pretend.Graphics;

namespace Pretend
{
    public interface ISettingsManager
    {
        bool Vsync { get; set; }
        ushort MaxFps { get; set; }
        Vector2i Resolution { get; set; }
        WindowMode WindowMode { get; set; }
        void Apply();
        void Reset();
    }

    public class SettingsManager : ISettingsManager
    {
        private readonly IWindow _window;
        private readonly IGraphicsContext _graphicsContext;

        public SettingsManager(IWindow window, IGraphicsContext graphicsContext)
        {
            _window = window;
            _graphicsContext = graphicsContext;
        }

        public bool Vsync { get; set; } = true;
        public ushort MaxFps { get; set; } = 60;
        public Vector2i Resolution { get; set; } = new Vector2i(1280, 720);
        public WindowMode WindowMode { get; set; } = WindowMode.Fullscreen;

        public void Apply()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
