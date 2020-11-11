using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            _target.RegisterServices<TestApplication>();
            _target.BuildContainer();

            var testApplication = _target.Create<IApplication>();

            Assert.IsTrue(testApplication is TestApplication);
        }

        [TestMethod]
        public void RegisterServices_RegistersDefinedInterfacesAndClasses()
        {
            _target.RegisterServices<TestApplication>();
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

    public interface IService
    {
    }

    public class Service : IService
    {
    }
}
