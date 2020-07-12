using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;

namespace Pretend.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private ApplicationRunner _target;

        private Mock<IApplication> _mockApplication;
        private Mock<IWindow> _mockWindow;
        private Mock<IEventDispatcher> _mockEventDispatcher;
        private Mock<ILog<ApplicationRunner>> _mockLog;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockApplication = new Mock<IApplication>(MockBehavior.Strict);
            _mockWindow = new Mock<IWindow>(MockBehavior.Strict);
            _mockEventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            _mockLog = new Mock<ILog<ApplicationRunner>>(MockBehavior.Strict);

            _target = new ApplicationRunner(_mockApplication.Object, _mockWindow.Object, _mockEventDispatcher.Object,
                _mockLog.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockApplication.VerifyAll();
            _mockWindow.VerifyAll();
            _mockEventDispatcher.VerifyAll();
            _mockLog.VerifyAll();
        }

        [TestMethod]
        public void Run_StartsEventLoop()
        {
            _mockLog.Setup(_ => _.Info("Hello World"));
            _mockEventDispatcher.Setup(_ => _.Register<WindowCloseEvent>(_target.OnClose));
            _mockApplication.Setup(_ => _.Start());
            _mockApplication.Setup(_ => _.Stop());
            _mockApplication.SetupGet(_ => _.Attributes).Returns(new WindowAttributes());
            _mockWindow.Setup(_ => _.Init(It.IsAny<WindowAttributes>()));
            _mockWindow.Setup(_ => _.Close());
            _mockWindow.Setup(_ => _.OnUpdate()).Callback(() => _target.OnClose(new WindowCloseEvent()));

            _target.Run();

            _mockLog.Verify(_ => _.Info("Hello World"), Times.Once);
        }
    }
}
