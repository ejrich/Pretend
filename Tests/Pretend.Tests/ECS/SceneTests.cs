using System.Net.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.ECS;
using Pretend.Graphics;

namespace Pretend.Tests.ECS
{
    [TestClass]
    public class SceneTests
    {
        private IScene _target;
        
        private Mock<I2DRenderer> _mockRenderer;
        private Mock<IEntityContainer> _mockEntityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRenderer = new Mock<I2DRenderer>(MockBehavior.Strict);
            _mockEntityContainer = new Mock<IEntityContainer>(MockBehavior.Strict);

            _target = new Scene(_mockRenderer.Object, _mockEntityContainer.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockRenderer.VerifyAll();
            _mockEntityContainer.VerifyAll();
        }

        [TestMethod]
        public void DeleteEntity_WhenNullComponent_Returns()
        {
            _target.DeleteEntity(null);
            
            _mockEntityContainer.Verify(_ => _.DeleteEntity(It.IsAny<IEntity>()), Times.Never);
        }

        [TestMethod]
        public void DeleteEntity_WithoutScriptComponent_DoesNotRunDetach()
        {
            var entity = new Mock<IEntity>();
            entity.Setup(_ => _.GetComponent<IScriptComponent>()).Returns((IScriptComponent) null);
            _mockEntityContainer.Setup(_ => _.DeleteEntity(entity.Object));

            _target.DeleteEntity(entity.Object);

            entity.Verify(_ => _.GetComponent<IScriptComponent>(), Times.Once);
        }

        [TestMethod]
        public void DeleteEntity_WithScriptComponent_RunsDetach()
        {
            var script = new Mock<IScriptComponent>();
            script.Setup(_ => _.Detach());
            var entity = new Mock<IEntity>();
            entity.Setup(_ => _.GetComponent<IScriptComponent>()).Returns(script.Object);
            _mockEntityContainer.Setup(_ => _.DeleteEntity(entity.Object));

            _target.DeleteEntity(entity.Object);

            entity.Verify(_ => _.GetComponent<IScriptComponent>(), Times.Once);
            script.Verify(_ => _.Detach(), Times.Once);
        }

        [TestMethod]
        public void AddComponent_WhenScriptComponent_AttachesScriptToEntity()
        {
            var entity = new Mock<IEntity>();
            var script = new Mock<IScriptComponent>(MockBehavior.Strict);
            script.Setup(_ => _.Attach());
            _mockEntityContainer.Setup(_ => _.AddComponent(entity.Object, script.Object));

            _target.AddComponent(entity.Object, script.Object);

            script.Verify(_ => _.Attach(), Times.Once);
            _mockEntityContainer.Verify(_ => _.AddComponent(entity.Object, script.Object), Times.Once);
        }

        [TestMethod]
        public void AddComponent_WhenOtherComponent_AttachesScriptToEntity()
        {
            var entity = new Mock<IEntity>();
            var position = new Mock<PositionComponent>(MockBehavior.Strict);
            _mockEntityContainer.Setup(_ => _.AddComponent(entity.Object, position.Object));

            _target.AddComponent(entity.Object, position.Object);

            _mockEntityContainer.Verify(_ => _.AddComponent(entity.Object, position.Object), Times.Once);
        }
    }
}
