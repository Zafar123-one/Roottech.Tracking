using System;
using System.Web;
using System.Web.Mvc;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Attributes;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Stationary.Controllers
{
    //[AuthorizeUser(AccessPage=AppType.Stationary)]
    public class MapController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Request.Form["selOrganizationByUser"]))
                SetCookies("UserOrgCode", Request.Form["selOrganizationByUser"]);
            return View();
        }

        public void SetCookies(string cName, string cValue)
        {
            HttpCookie myCookie = Request.Cookies[cName] ?? new HttpCookie(cName);
            myCookie.Values[cName] = cValue;
            myCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(myCookie);
        }
    }
}
