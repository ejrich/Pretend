using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;
using Pretend.Graphics;
using Pretend.Layers;

namespace Pretend.Tests.Layers
{
    [TestClass]
    public class LayerContainerTests
    {
        private ILayerContainer _target;

        private Mock<IEventDispatcher> _mockEventDispatcher;
        private Mock<IFactory> _mockFactory;
        private Mock<IRenderContext> _mockRenderContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockEventDispatcher = new Mock<IEventDispatcher>(MockBehavior.Strict);
            _mockEventDispatcher.Setup(_ => _.Register(It.IsAny<Action<IEvent>>()));
            _mockFactory = new Mock<IFactory>(MockBehavior.Strict);
            _mockRenderContext = new Mock<IRenderContext>(MockBehavior.Strict);

            _target = new LayerContainer(_mockEventDispatcher.Object, _mockFactory.Object, _mockRenderContext.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockEventDispatcher.VerifyAll();
            _mockRenderContext.VerifyAll();
        }

        [TestMethod]
        public void PushLayer_AddsNewLayerToList()
        {
            _mockRenderContext.Setup(_ => _.ClearDepth());

            var layer = new TestLayer();
            _target.PushLayer(layer);

            _target.Update(0);

            Assert.IsTrue(layer.UpdateCalled);
            Assert.IsTrue(layer.RenderCalled);
        }

        [TestMethod]
        public void PushLayerT_AddsNewLayerOfTypeToList()
        {
            var layer = new TestLayer();
            _mockFactory.Setup(_ => _.Create<TestLayer>()).Returns(layer);
            _mockRenderContext.Setup(_ => _.ClearDepth());

            _target.PushLayer<TestLayer>();

            _target.Update(0);

            Assert.IsTrue(layer.UpdateCalled);
            Assert.IsTrue(layer.RenderCalled);
        }

        [TestMethod]
        public void SetLayerOrder_OrdersAndCreatesLayers()
        {
            var layer1 = new TestLayer();
            var layer2 = new TestLayer2();
            _mockFactory.Setup(_ => _.Create<ILayer>(typeof(TestLayer))).Returns(layer1);
            _mockFactory.Setup(_ => _.Create<ILayer>(typeof(TestLayer2))).Returns(layer2);
            _mockRenderContext.Setup(_ => _.ClearDepth());

            _target.SetLayerOrder(typeof(TestLayer), typeof(TestLayer2));
            // Assign new layers
            _target.Update(0);
            // Run update on new layers
            _target.Update(0);

            Assert.IsTrue(layer1.UpdateCalled);
            Assert.IsTrue(layer1.RenderCalled);
            Assert.IsTrue(layer2.UpdateCalled);
            Assert.IsTrue(layer2.RenderCalled);
        }

        [TestMethod]
        public void Update_OnlyUpdatesUnPausedLayers()
        {
            _mockRenderContext.Setup(_ => _.ClearDepth());

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
            var layer = new TestLayer();
            _target.PushLayer(layer);
            _target.RemoveLayer<TestLayer>();

            _target.Update(0);

            Assert.IsFalse(layer.UpdateCalled);
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
        
        private class TestLayer2 : TestLayer
        {
        }

        public class TestEvent : IEvent
        {
            public bool Processed { get; set; }
        }
    }
}
