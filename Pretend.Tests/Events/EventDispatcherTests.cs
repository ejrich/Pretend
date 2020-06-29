using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;

namespace Pretend.Tests.Events
{
    [TestClass]
    public class EventDispatcherTests
    {
        private IEventDispatcher _target;

        private Mock<TestEventHandler> _mockEventHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockEventHandler = new Mock<TestEventHandler>{ CallBase = true };
            var eventHandlers = new List<IEventHandler>
            {
                _mockEventHandler.Object
            };

            _target = new EventDispatcher(eventHandlers);
        }

        [TestMethod]
        public void Dispatch_SendsToEventHandler()
        {
            var evnt = new TestEvent();

            _target.DispatchEvent(evnt);

            _mockEventHandler.Verify(_ => _.Handle(evnt), Times.Once);
        }

        public class TestEvent : IEvent
        {
        }

        public class TestEventHandler : IEventHandler<TestEvent>
        {
            public override void Handle(TestEvent evnt)
            {
                Console.WriteLine("Hello World");
            }
        }
    }
}
