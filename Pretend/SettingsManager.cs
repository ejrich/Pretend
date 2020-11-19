using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OpenTK.Mathematics;
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

    public interface ISettingsManager<out T> where T : Settings, new()
    {
        T Settings { get; }
        void Apply(Action<T> apply = null);
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
                WriteSettings();
            }
            else
            {
                ReadSettings();
            }
        }
        
        public T Settings { get; private set; }

        public void Apply(Action<T> apply = null)
        {
            _graphicsContext.Vsync = Settings.Vsync;
            _window.MaxFps = Settings.MaxFps;
            _window.Resolution = new Vector2i(Settings.ResolutionX, Settings.ResolutionY);
            _window.WindowMode = Settings.WindowMode;
            _window.MouseGrab = Settings.MouseGrab;

            apply?.Invoke(Settings);

            WriteSettings();
        }

        public void Reset()
        {
            ReadSettings();
        }

        private void ReadSettings()
        {
            // Try to read the settings.json file
            try
            {
                var settingsFile = File.ReadAllText(SettingsFile);
                Settings = JsonSerializer.Deserialize<T>(settingsFile);
            }
            // If unable to read from the settings.json file, set the settings back to default
            catch
            {
                Settings = new T();
                WriteSettings();
            }
        }

        private void WriteSettings()
        {
            Task.Run(() =>
            {
                var settingsString = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, settingsString);
            });
        }
    }
}
