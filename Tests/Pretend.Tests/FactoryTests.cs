using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pretend.Tests
{
    [TestClass]
    public class FactoryTests
    {
        private Factory _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _target = new Factory();
        }

        [TestMethod]
        public void RegisterServices_RegistersDefaultServices()
        {
            _target.RegisterServices<TestApplication, TestWindowsAttributes>();
            _target.BuildContainer();

            var testApplication = _target.Create<IApplication>();
            var TestWindowsAttributes = _target.Create<IWindowAttributesProvider>();

            Assert.IsTrue(testApplication is TestApplication);
            Assert.IsTrue(TestWindowsAttributes is TestWindowsAttributes);
        }

        [TestMethod]
        public void RegisterServices_RegistersDefinedInterfacesAndClasses()
        {
            _target.RegisterServices<TestApplication, TestWindowsAttributes>();
            _target.BuildContainer();

            var serviceInterface = _target.Create<IService>();
            var serviceClass = _target.Create<Service>();

            Assert.IsTrue(serviceInterface is Service);
            Assert.IsTrue(serviceClass is Service);
        }
    }

    public class TestApplication : IApplication
    {
    }

    public class TestWindowsAttributes : IWindowAttributesProvider
    {
        public string Title => "Test";
    }

    public interface IService
    {
    }

    public class Service : IService
    {
    }
}
