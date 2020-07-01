using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pretend.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private IApplicationRunner _target;

        private Mock<IApplication> _mockApplication;
        private Mock<ILog<ApplicationRunner>> _mockLog;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockApplication = new Mock<IApplication>(MockBehavior.Strict);
            _mockLog = new Mock<ILog<ApplicationRunner>>(MockBehavior.Strict);

            _target = new ApplicationRunner(_mockApplication.Object, _mockLog.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockApplication.VerifyAll();
            _mockLog.VerifyAll();
        }

        [TestMethod]
        public void Run_StartsEventLoop()
        {
            _mockLog.Setup(_ => _.Info("Hello World"));
            _mockApplication.Setup(_ => _.Start());
            _mockApplication.Setup(_ => _.Stop());

            _target.Run();

            _mockLog.Verify(_ => _.Info("Hello World"), Times.Once);
        }
    }
}
