using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher
{
    public class AreaHttpControllerSelector : DefaultHttpControllerSelector
    {
        private const string AreaRouteVariableName = "area";

        private readonly HttpConfiguration _configuration;
        private readonly Lazy<ConcurrentDictionary<string, Type>> _apiControllerTypes;

        public AreaHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _apiControllerTypes = new Lazy<ConcurrentDictionary<string, Type>>(GetControllerTypes);
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            return GetApiController(request);
        }

        private static string GetAreaName(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            if (data.Route.DataTokens == null)
            {
                return null;
            }
            object areaName;
            return data.Route.DataTokens.TryGetValue(AreaRouteVariableName, out areaName) ? areaName.ToString() : null;
        }

        /*private static string GetAreaName(HttpRequestMessage request)
        {
            var data = request.GetRouteData();
            return !data.Values.ContainsKey(AreaRouteVariableName) ? null : 
                data.Values[AreaRouteVariableName].ToString();
        }*/

        private static ConcurrentDictionary<string, Type> GetControllerTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = System.Reflection.Assembly.GetCallingAssembly().GetTypes().Where(t => !t.IsAbstract && t.Name.EndsWith("Controller") && typeof(IHttpController).IsAssignableFrom(t)).ToDictionary(t => t.FullName, t => t);

			//var types = assemblies
   //             .SelectMany(a => a
   //                 .GetTypes().Where(t =>
   //                     !t.IsAbstract &&
   //                     t.Name.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase) &&
   //                     typeof(IHttpController).IsAssignableFrom(t)))
   //             .ToDictionary(t => t.FullName, t => t);

            return new ConcurrentDictionary<string, Type>(types);
        }

        private HttpControllerDescriptor GetApiController(HttpRequestMessage request)
        {
            var areaName = GetAreaName(request);
            var controllerName = GetControllerName(request);
            var controllerType = GetControllerType(areaName, controllerName);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            return new HttpControllerDescriptor(_configuration, controllerName, controllerType);
        }

        private Type GetControllerType(string areaName, string controllerName)
        {
            var query = _apiControllerTypes.Value.AsEnumerable();
            query = string.IsNullOrEmpty(areaName) ? query.WithoutAreaName() : query.ByAreaName(areaName);
            return query.ByControllerName(controllerName).Select(x => x.Value).Single();
        }
    }
}
