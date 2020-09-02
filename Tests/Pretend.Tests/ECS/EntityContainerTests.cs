using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.ECS;

namespace Pretend.Tests.ECS
{
    [TestClass]
    public class EntityContainerTests
    {
        private IEntityContainer _target;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _target = new EntityContainer();
        }

        [TestMethod]
        public void AddComponent_AddsComponentToEntityAndContainer()
        {
            var entity = new Mock<IEntity>(MockBehavior.Strict);
            var component = new Mock<IScriptComponent>();
            entity.Setup(_ => _.AddComponent(component.Object));

            _target.AddComponent(entity.Object, component.Object);

            Assert.IsTrue(_target.GetComponents<IScriptComponent>().Any());
            entity.Verify(_ => _.AddComponent(component.Object), Times.Once);
        }

        [TestMethod]
        public void DeleteEntity_DeletesTheEntityAndAllComponents()
        {
            var scriptComponent = new Mock<IScriptComponent>();
            var positionComponent = new PositionComponent();

            var entity = _target.CreateEntity();
            _target.AddComponent(entity, scriptComponent.Object);
            _target.AddComponent(entity, positionComponent);
            _target.DeleteEntity(entity);

            Assert.IsFalse(_target.GetComponents<IScriptComponent>().Any());
            Assert.IsFalse(_target.GetComponents<PositionComponent>().Any());
            Assert.IsFalse(_target.Entities.Any());
        }
    }
}
