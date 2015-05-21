using System.Linq;
using Castle.Windsor;
using NUnit.Framework;
using OrbitOne.BuildScreen.DependencyInjection;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.RestApiServiceTest
{
    [TestFixture]
    public class RestApiServiceTest
    {
        //private IWindsorContainer _windsorContainer;

/*        [SetUp]
        public void SetUp()
        {
            _windsorContainer = Bootstrapper.Bootstrap();
        }

        [Test]
        public void GetBuilds_Test()
        {
            var serviceFacade = _windsorContainer.Resolve<IServiceFacade>();
            var result = serviceFacade.GetBuilds();

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Any(r => !string.IsNullOrEmpty(r.Status)));
        }*/
    }
}

