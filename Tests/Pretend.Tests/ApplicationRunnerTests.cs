using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;
using Pretend.Layers;
using Pretend.Graphics;

namespace Pretend.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private ApplicationRunner _target;

        private Mock<IApplication> _mockApplication;
        private Mock<IWindow> _mockWindow;
        private Mock<IEventDispatcher> _mockEventDispatcher;
        private Mock<ILayerContainer> _mockLayerContainer;
        private Mock<IRenderContext> _mockRenderContext;
        private Settings _settings;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockApplication = new Mock<IApplication>(MockBehavior.Strict);
            _mockWindow = new Mock<IWindow>(MockBehavior.Strict);
            _mockEventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            _mockLayerContainer = new Mock<ILayerContainer>(MockBehavior.Strict);
            _mockRenderContext = new Mock<IRenderContext>(MockBehavior.Strict);
            _settings = new Settings();

            _target = new ApplicationRunner(_mockApplication.Object, _mockWindow.Object, _mockEventDispatcher.Object,
                _mockLayerContainer.Object, _mockRenderContext.Object, _settings);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockApplication.VerifyAll();
            _mockWindow.VerifyAll();
            _mockEventDispatcher.VerifyAll();
            _mockLayerContainer.VerifyAll();
            _mockRenderContext.VerifyAll();
        }

        [TestMethod]
        public void Run_StartsEventLoop()
        {
            _mockEventDispatcher.Setup(_ => _.Register<WindowCloseEvent>(_target.OnClose));
            _mockEventDispatcher.Setup(_ => _.Register<WindowResizeEvent>(_target.OnResize));
            _mockApplication.Setup(_ => _.Start());
            _mockApplication.Setup(_ => _.Stop());
            _mockWindow.Setup(_ => _.Init("Test", _settings));
            _mockWindow.Setup(_ => _.GetTimestep()).Returns(0);
            _mockWindow.Setup(_ => _.Close());
            _mockWindow.Setup(_ => _.OnUpdate()).Callback(() => _target.OnClose(new WindowCloseEvent()));
            _mockLayerContainer.Setup(_ => _.Update(It.IsAny<float>()));
            _mockLayerContainer.Setup(_ => _.RemoveAll());
            _mockRenderContext.Setup(_ => _.Clear());

            _target.Run("Test");
        }
    }
}
