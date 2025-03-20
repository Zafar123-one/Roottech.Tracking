using System.Web.Mvc;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher;

namespace Roottech.Tracking.RTM.UI.Web.Areas.SMIM
{
    public class SMIMAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "SMIM"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapHttpRoute("Area_defaultApi", "api/SMIM/{controller}/{action}/{id}", 
                new { action = "Get", id = UrlParameter.Optional });

            context.MapRoute(
                "Area_default",
                "SMIM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Roottech.Tracking.RTM.UI.Web.Areas.SMIM.Controllers" }
                );
        }
    }
}
