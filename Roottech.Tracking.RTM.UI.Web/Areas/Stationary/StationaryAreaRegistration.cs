using System.Web.Mvc;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Stationary
{
    public class StationaryAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Stationary"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapHttpRoute("Stationary_defaultApi", "api/Stationary/{controller}/{action}/{id}", 
                new { action = "Get", id = UrlParameter.Optional });

            context.MapRoute(
                "Stationary_default",
                "Stationary/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Roottech.Tracking.RTM.UI.Web.Areas.Stationary.Controllers" }
            );
        }
    }
}
