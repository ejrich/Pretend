using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OpenTK.Mathematics;
using Pretend.Graphics;

namespace Pretend.Tests
{
    [TestClass]
    public class SettingsManagerTests
    {
        private ISettingsManager<Settings> _target;

        private Mock<IWindow> _mockWindow;
        private Mock<IGraphicsContext> _mockGraphicsContext;

        [TestInitialize]
        public void TestInitialize()
        {
            File.Delete(SettingsManager<Settings>.SettingsFile);

            _mockWindow = new Mock<IWindow>(MockBehavior.Strict);
            _mockGraphicsContext = new Mock<IGraphicsContext>(MockBehavior.Strict);

            _target = new SettingsManager<Settings>(_mockWindow.Object, _mockGraphicsContext.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockWindow.VerifyAll();
            _mockGraphicsContext.VerifyAll();
        }

        [TestMethod]
        public void Reset_RevertsChangedSettingsToDefault()
        {
            var originalVsync = _target.Settings.Vsync;
            _target.Settings.Vsync = !_target.Settings.Vsync;

            var originalFps = _target.Settings.MaxFps;
            _target.Settings.MaxFps = 144;

            var originalX = _target.Settings.ResolutionX;
            _target.Settings.ResolutionX = 1920;

            var originalY= _target.Settings.ResolutionY;
            _target.Settings.ResolutionY = 1080;

            var originalWindowMode = _target.Settings.WindowMode;
            _target.Settings.WindowMode = WindowMode.Fullscreen | WindowMode.Borderless;

            var originalMouseGrab = _target.Settings.MouseGrab;
            _target.Settings.MouseGrab = !_target.Settings.MouseGrab;

            _target.Reset();

            Assert.AreEqual(originalVsync, _target.Settings.Vsync);
            Assert.AreEqual(originalFps, _target.Settings.MaxFps);
            Assert.AreEqual(originalX, _target.Settings.ResolutionX);
            Assert.AreEqual(originalY, _target.Settings.ResolutionY);
            Assert.AreEqual(originalWindowMode, _target.Settings.WindowMode);
            Assert.AreEqual(originalMouseGrab, _target.Settings.MouseGrab);
        }

        [TestMethod]
        public void Apply_SetsValuesOnWindowAndGraphicsContext()
        {
            _target.Settings.Vsync = !_target.Settings.Vsync;
            _target.Settings.MaxFps = 144;
            _target.Settings.ResolutionX = 1920;
            _target.Settings.ResolutionY = 1080;
            _target.Settings.WindowMode = WindowMode.Fullscreen | WindowMode.Borderless;
            _target.Settings.MouseGrab = !_target.Settings.MouseGrab;

            _mockGraphicsContext.SetupSet(_ => _.Vsync = _target.Settings.Vsync);
            _mockWindow.SetupSet(_ => _.MaxFps = _target.Settings.MaxFps);
            _mockWindow.SetupSet(_ => _.Resolution = new Vector2i(_target.Settings.ResolutionX, _target.Settings.ResolutionY));
            _mockWindow.SetupSet(_ => _.WindowMode = _target.Settings.WindowMode);
            _mockWindow.SetupSet(_ => _.MouseGrab = _target.Settings.MouseGrab);

            _target.Apply();
        }
    }
}
