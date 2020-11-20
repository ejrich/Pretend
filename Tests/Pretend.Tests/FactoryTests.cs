using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pretend.Tests
{
    [TestClass]
    public class FactoryTests
    {
        private Factory _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _target = new Factory();
        }

        [TestMethod]
        public void RegisterServices_RegistersDefaultServices()
        {
            _target.RegisterServices<TestApplication, Settings>();
            _target.BuildContainer();

            var testApplication = _target.Create<IApplication>();

            Assert.IsTrue(testApplication is TestApplication);
        }

        [TestMethod]
        public void RegisterServices_RegistersDefinedInterfacesAndClasses()
        {
            _target.RegisterServices<TestApplication, Settings>();
            _target.BuildContainer();

            var serviceInterface = _target.Create<IService>();
            var serviceClass = _target.Create<Service>();

            Assert.IsTrue(serviceInterface is Service);
            Assert.IsTrue(serviceClass is Service);
        }

        [TestMethod]
        public void RegisterServices_RegistersCustomSettings()
        {
            try
            {
                _target.RegisterServices<TestApplication, CustomSettings>();
                _target.BuildContainer();

                var settingsManager = _target.Create<ISettingsManager<Settings>>();
                settingsManager.Settings.Vsync = false;
                var customSettingsManager = _target.Create<ISettingsManager<CustomSettings>>();
                customSettingsManager.Settings.CustomSetting = false;
                var settings = _target.Create<Settings>();
                var customSettings = (CustomSettings)_target.Create<Settings>();

                Assert.AreSame(settingsManager.Settings, settings);
                Assert.AreSame(customSettingsManager.Settings, customSettings);

                Assert.IsFalse(settings.Vsync);
                Assert.IsFalse(customSettings.Vsync);
                Assert.IsFalse(customSettings.CustomSetting);
            }
            finally
            {
                File.Delete(SettingsManager<Settings>.SettingsFile);
            }
        }
    }

    public class TestApplication : IApplication
    {
    }

    public interface IService
    {
    }

    public class Service : IService
    {
    }

    public class CustomSettings : Settings
    {
        public bool CustomSetting { get; set; } = true;
    }
}
