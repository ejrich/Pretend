using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pretend.Events;

namespace Pretend.Tests.Events
{
    [TestClass]
    public class EventDispatcherTests
    {
        private IEventDispatcher _target;

        [TestInitialize]
        public void TestInitialize()
        {
            var eventHandlers = new List<IEventHandler>
            {
                new TestEventHandler()
            };

            _target = new EventDispatcher(eventHandlers);
        }

        [TestMethod]
        public void Dispatch_SendsToEventHandler()
        {
            var evnt = new TestEvent();

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                _target.DispatchEvent(evnt);

                var expected = string.Format("Hello World{0}", Environment.NewLine);
                Assert.AreEqual(expected, sw.ToString());
            }
        }

        public class TestEvent : IEvent
        {
            public bool Processed { get; set; }
        }

        public class TestEventHandler : BaseEventHandler<TestEvent>
        {
            public override void Handle(TestEvent evnt)
            {
                Console.WriteLine("Hello World");
            }
        }
    }
}
