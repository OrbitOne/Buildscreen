using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using OrbitOne.BuildScreen.Models;
using OrbitOne.BuildScreen.Services;

namespace OrbitOne.BuildScreen.Controllers
{
    public class BuildscreenApiController : Controller
    {

        private readonly IServiceFacade _serviceFacade;

        public BuildscreenApiController(IServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;
        }

        [OutputCache(CacheProfile = "CacheDuration")]
        public ActionResult GetBuilds()
        {
            List<BuildInfoDto> allBuilds;
            try
            {
                allBuilds = _serviceFacade.GetBuilds();
            }
            catch (ObjectNotFoundException oe)
            {
                return new HttpStatusCodeResult(512, oe.Message);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                return new HttpStatusCodeResult(500, e.Message);
            }
            LogService.WriteInfo(allBuilds.Count + " builds were returned.");
            return Json(allBuilds, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "CacheDuration", VaryByParam = "*")]
        public ActionResult GetBuildsSince(string dateString)
        {
            List<BuildInfoDto> allBuilds;
            try
            {
                allBuilds = _serviceFacade.GetBuilds(dateString);
            }
            catch (ObjectNotFoundException oe)
            {
                return new HttpStatusCodeResult(512, oe.Message);
            }
            catch (Exception e)
            {
                LogService.WriteError(e);
                return new HttpStatusCodeResult(500, e.Message);
            }
            LogService.WriteInfo(allBuilds.Count + " builds were returned.");
            return Json(allBuilds, JsonRequestBehavior.AllowGet);
        }
    }
}