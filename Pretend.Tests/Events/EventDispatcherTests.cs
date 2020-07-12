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
        public void Dispatch_SendsToRegisteredEventHandler()
        {
            var passed = false;

            _target.Register(_ => passed = true);

            _target.DispatchEvent(new TestEvent());

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void Dispatch_SendsToRegisteredTypeEventHandler()
        {
            var passed = false;

            _target.Register<TestEvent>(_ => passed = true);

            _target.DispatchEvent(new TestEvent());

            Assert.IsTrue(passed);
        }

        public class TestEvent : IEvent
        {
            public bool Processed { get; set; }
        }
    }
}
