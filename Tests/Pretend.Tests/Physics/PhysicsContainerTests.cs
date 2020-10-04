using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.ECS;
using Pretend.Physics;

namespace Pretend.Tests.Physics
{
    [TestClass]
    public class PhysicsContainerTests
    {
        private IPhysicsContainer _target;

        private Mock<IEntityContainer> _mockEntityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockEntityContainer = new Mock<IEntityContainer>();

            _target = new PhysicsContainer();
        }

        [TestMethod]
        public void Simulate()
        {
            // TODO
            Assert.Fail("Tests not yet implemented");
        }
    }
}
