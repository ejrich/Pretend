using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private IApplication _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _target = new Application();
        }

        [TestMethod]
        public void Run_ExecutesDesiredCode()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                _target.Run();

                var expected = string.Format("Hello World{0}", Environment.NewLine);
                Assert.AreEqual(expected, sw.ToString());
            }
        }
    }
}
