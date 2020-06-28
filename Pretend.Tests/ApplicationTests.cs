using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pretend.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private IApplication _target;

        private Mock<ILog<Application>> _mockLog;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLog = new Mock<ILog<Application>>(MockBehavior.Strict);

            _target = new Application(_mockLog.Object);
        }

        [TestMethod]
        public void Run_ExecutesDesiredCode()
        {
            _mockLog.Setup(_ => _.Info("Hello World"));

            _target.Run();

            _mockLog.Verify(_ => _.Info("Hello World"), Times.Once);
        }
    }
}
