using System.Web.Mvc;

namespace Roottech.Tracking.RTM.UI.Web.Areas.SMIM.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Vehicle()
        {
            return View();
        }

        public ActionResult FuelDispensionUnit()
        {
            return View();
        }

        public ActionResult Stationary()
        {
            return View();
        }
    }
}
