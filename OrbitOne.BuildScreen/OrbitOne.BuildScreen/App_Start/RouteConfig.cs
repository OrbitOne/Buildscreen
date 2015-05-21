using System.Web.Mvc;
using System.Web.Routing;

namespace OrbitOne.BuildScreen
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{dateString}",
                defaults: new { controller = "Home", action = "Index", dateString = UrlParameter.Optional }
            );
        }
    }
}