using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            _target = new EventDispatcher();
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
    }
}
