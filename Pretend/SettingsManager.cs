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
        bool MouseGrab { get; set; }
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
        public ushort MaxFps { get; set; }
        public Vector2i Resolution { get; set; } = new Vector2i(1920, 1080);
        public WindowMode WindowMode { get; set; }
        public bool MouseGrab { get; set; }

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
