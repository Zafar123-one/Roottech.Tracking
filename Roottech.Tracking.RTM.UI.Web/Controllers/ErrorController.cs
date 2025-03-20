using System.Web.Mvc;

namespace Roottech.Tracking.RTM.UI.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Unauthorised()
        {
            return View();
        }

    }
}
