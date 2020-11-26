using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pretend.ECS;
using Pretend.Physics;

namespace Pretend.Tests.Physics
{
    [TestClass]
    public class PhysicsContainerTests
    {
        private IPhysicsContainer _target;

        private IEntityContainer _entityContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            _entityContainer = new EntityContainer();

            _target = new PhysicsContainer();
        }

        [TestMethod]
        public void Simulate_GravityMovesEntityDown()
        {
            var entity = _entityContainer.CreateEntity();
            var position = new PositionComponent();
            _entityContainer.AddComponent(entity, position);
            _entityContainer.AddComponent(entity, new PhysicsComponent());

            _target.Gravity = new Vector3(0, -100, 0);
            _target.Simulate(1, _entityContainer);

            Assert.AreEqual(-75, position.Position.Y);
        }

        [TestMethod]
        public void Simulate_GravityMoveEntityOnTopOfFixedEntity()
        {
            const uint dimension = 20;
            
            var entity = _entityContainer.CreateEntity();
            var position = new PositionComponent();
            _entityContainer.AddComponent(entity, position);
            _entityContainer.AddComponent(entity, new SizeComponent { Height = dimension, Width = dimension });
            _entityContainer.AddComponent(entity, new PhysicsComponent());

            entity = _entityContainer.CreateEntity();
            _entityContainer.AddComponent(entity, new PositionComponent { Position = new Vector3(0, -50, 0) });
            _entityContainer.AddComponent(entity, new SizeComponent { Height = dimension, Width = dimension });
            _entityContainer.AddComponent(entity, new PhysicsComponent { Fixed = true });
            
            _target.Gravity = new Vector3(0, -100, 0);
            _target.Simulate(1, _entityContainer);

            Assert.AreEqual(-50 + dimension, position.Position.Y);
        }

        [TestMethod]
        public void Simulate_VelocityMovesOnGround()
        {
            var entity = _entityContainer.CreateEntity();
            var position = new PositionComponent();
            _entityContainer.AddComponent(entity, position);
            _entityContainer.AddComponent(entity, new SizeComponent { Height = 20, Width = 20 });
            _entityContainer.AddComponent(entity, new PhysicsComponent { Velocity = new Vector3(100, 0, 0) });

            entity = _entityContainer.CreateEntity();
            _entityContainer.AddComponent(entity, new PositionComponent { Position = new Vector3(0, -15, 0) });
            _entityContainer.AddComponent(entity, new SizeComponent { Height = 10, Width = 500 });
            _entityContainer.AddComponent(entity, new PhysicsComponent { Fixed = true });
            
            _target.Gravity = new Vector3(0, -100, 0);
            _target.Simulate(0.016f, _entityContainer);

            // There will be some error when the time step is this small
            Assert.IsTrue(position.Position.X - 1.6f < 0.000001f);
        }
    }
}
