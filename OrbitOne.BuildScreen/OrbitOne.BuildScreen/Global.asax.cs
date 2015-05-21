using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using OrbitOne.BuildScreen.Configuration;
using OrbitOne.BuildScreen.DependencyInjection;

namespace OrbitOne.BuildScreen
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private static IWindsorContainer _windsorContainer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrap();
            ServiceConfiguration.ConfigurationListChangedHandler += (sender, args) => Bootstrap();
        }

        public void Bootstrap()
        {
            _windsorContainer = Bootstrapper.Bootstrap();
        }


        protected void Application_End()
        {
            _windsorContainer.Dispose();
        }

    }
}