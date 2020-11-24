using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;
using Pretend.Layers;

namespace Pretend.Tests.Layers
{
    [TestClass]
    public class LayerContainerTests
    {
        private ILayerContainer _target;

        private Mock<IEventDispatcher> _mockEventDispatcher;
        private Mock<IFactory> _mockFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockEventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            _mockEventDispatcher.Setup(_ => _.Register(It.IsAny<Action<IEvent>>()));
            _mockFactory = new Mock<IFactory>(MockBehavior.Strict);

            _target = new LayerContainer(_mockEventDispatcher.Object, _mockFactory.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockEventDispatcher.VerifyAll();
        }

        [TestMethod]
        public void PushLayer_AddsNewLayerToList()
        {
            var layer = new TestLayer();
            _target.PushLayer(layer);

            _target.Update(0);

            Assert.IsTrue(layer.UpdateCalled);
            Assert.IsTrue(layer.RenderCalled);
        }

        [TestMethod]
        public void PushLayerT_AddsNewLayerOfTypeToList()
        {
            Assert.Fail("NYI");
        }

        [TestMethod]
        public void SetLayerOrder_OrdersAndCreatesLayers()
        {
            Assert.Fail("NYI");
        }

        [TestMethod]
        public void Update_OnlyUpdatesUnPausedLayers()
        {
            var layer = new TestLayer { Paused = true };
            _target.PushLayer(layer);

            _target.Update(0);

            Assert.IsFalse(layer.UpdateCalled);
            Assert.IsTrue(layer.RenderCalled);
        }

        [TestMethod]
        public void RemoveLayer_RemovesLayerFromList()
        {
            var layer = new TestLayer();
            _target.PushLayer(layer);
            _target.RemoveLayer(layer);

            _target.Update(0);

            Assert.IsFalse(layer.UpdateCalled);
        }

        [TestMethod]
        public void RemoveLayerT_RemovesFirstLayerOfType()
        {
            Assert.Fail("NYI");
        }

        private class TestLayer : ILayer
        {
            public bool UpdateCalled { get; private set; }
            public bool RenderCalled { get; private set; }
            public bool Paused { get; set; }
            public void Update(float timeStep) => UpdateCalled = true;
            public void Render() => RenderCalled = true;
            public void HandleEvent(IEvent evnt) {}
        }

        public class TestEvent : IEvent
        {
            public bool Processed { get; set; }
        }
    }
}
