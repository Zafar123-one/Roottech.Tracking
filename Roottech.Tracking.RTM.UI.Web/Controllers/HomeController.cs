using System.Web.Mvc;

namespace Roottech.Tracking.RTM.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToRoute("Area_default", new { controller = "MainPage" });
        }
    }
}
