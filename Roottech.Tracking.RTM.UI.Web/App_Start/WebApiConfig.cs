using System.Web.Http;

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /*
            // Controller Only
            // To handle routes like `/api/VTRouting`
            config.Routes.MapHttpRoute(
                name: "ControllerOnly",
                routeTemplate: "api/{controller}"
            );

            // Controller with ID
            // To handle routes like `/api/VTRouting/1`
            config.Routes.MapHttpRoute(
                name: "ControllerAndId",
                routeTemplate: "api/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
            );

            // Controllers with Actions
            // To handle routes like `/api/VTRouting/route`
            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/{controller}/{action}"
            );*/
            /*
            config.Routes.MapHttpRoute(
                name: "Area_ControllerOnly",
                routeTemplate: "api/{area}/{controller}"
                );

            config.Routes.MapHttpRoute(
                name: "Area_ControllerAndId",
                routeTemplate: "api/{area}/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
                );

            config.Routes.MapHttpRoute(
                name: "Area_ControllerAndAction",
                routeTemplate: "api/{area}/{controller}/{action}"
                );*/
        }
    }
}
