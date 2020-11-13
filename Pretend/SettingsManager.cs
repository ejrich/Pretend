using System.IO;
using System.Text.Json;
using Pretend.Graphics;

namespace Pretend
{
    public class Settings
    {
        public bool Vsync { get; set; } = true;
        public ushort MaxFps { get; set; }
        public ushort ResolutionX { get; set; } = 1280;
        public ushort ResolutionY { get; set; } = 720;
        public WindowMode WindowMode { get; set; }
        public bool MouseGrab { get; set; }
    }

    public interface ISettingsManager<out T> where T : Settings
    {
        T Settings { get; }
        void Apply();
        void Reset();
    }

    public class SettingsManager<T> : ISettingsManager<T> where T : Settings, new()
    {
        private readonly IWindow _window;
        private readonly IGraphicsContext _graphicsContext;

        private const string SettingsFile = "settings.json";

        public SettingsManager(IWindow window, IGraphicsContext graphicsContext)
        {
            _window = window;
            _graphicsContext = graphicsContext;

            if (!File.Exists(SettingsFile))
            {
                Settings = new T();
                var settingsString = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, settingsString);
            }
            else
            {
                var settingsFile= File.ReadAllText(SettingsFile);
                Settings = JsonSerializer.Deserialize<T>(settingsFile);
            }
        }
        
        public T Settings { get; }

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
