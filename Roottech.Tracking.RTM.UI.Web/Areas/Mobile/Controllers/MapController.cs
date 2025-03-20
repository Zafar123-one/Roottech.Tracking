using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Internal;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Attributes;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Mobile.Controllers
{
    [AuthorizeUser(AccessPage = AppType.Mobile)]
    public class MapController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Request.Form["selOrganizationByUserForMobile"]))
                SetCookies("UserOrgCode", Request.Form["selOrganizationByUserForMobile"]);
            return View();
        }

        public void SetCookies(string cName, string cValue)
        {
            HttpCookie myCookie = Request.Cookies[cName] ?? new HttpCookie(cName);
            myCookie.Values[cName] = cValue;
            myCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(myCookie);
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string filePath)
        {
            
            string foldername = filePath.Substring(0, filePath.LastIndexOf("/"));//$"~/Images/Organizations/{orgCode}/";
            string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            //if (!companyCode.IsNullOrEmpty())
            //    foldername = $"{foldername}/{companyCode}/";
            if (!Directory.Exists(Server.MapPath(foldername)))
                Directory.CreateDirectory(Server.MapPath(foldername));

            if (file == null || file.ContentLength <= 0)
                return Json(new
                {
                    success = false
                }, JsonRequestBehavior.DenyGet);

            file.SaveAs(Path.Combine(Server.MapPath(foldername), fileName));//Path.GetFileName(file.FileName));
            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
