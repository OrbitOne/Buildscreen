using System.Web.Mvc;

namespace OrbitOne.BuildScreen.Controllers
{
    public class HomeController : Controller
    {
        /**
         * The only purpose of this Controller is to load the Index view.
         * Then AngularJS takes over, and loads erverything and handles routing.
         **/
        public ActionResult Index()
        {
           return View();
        }

    }
}
