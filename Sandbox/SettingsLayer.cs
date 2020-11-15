using Pretend;
using Pretend.Events;
using Pretend.Layers;

namespace Sandbox
{
    public class SettingsLayer : ILayer
    {
        private readonly ISettingsManager<Settings> _settingsManager;

        public SettingsLayer(ISettingsManager<Settings> settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Update(float timeStep)
        {
        }

        public void HandleEvent(IEvent evnt)
        {
        }
    }
}
