using System.Web;
using System.Web.Mvc;

namespace Roottech.Tracking.RTM.Boundries.UI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}