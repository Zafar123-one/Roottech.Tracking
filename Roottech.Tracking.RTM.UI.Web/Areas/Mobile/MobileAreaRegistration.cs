using System.Web.Mvc;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Mobile"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapHttpRoute("Mobile_defaultApi", "api/Mobile/{controller}/{action}/{id}",
                new { action = "Get", id = UrlParameter.Optional });

            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }, 
                new[] { "Roottech.Tracking.RTM.UI.Web.Areas.Mobile.Controllers" }
            );
        }
    }
}
