using System;
using System.Linq;
using System.Web;
using Castle.MicroKernel;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure
{
    public class SessionFactoryHandlerSelector : IHandlerSelector
    {
        public bool HasOpinionAbout(string key, Type service)
        {
            return service == typeof(NHibernate.ISessionFactory);
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            if (!handlers.Any()) return null;
            //var dbName = HttpContext.Current.Session;
            /*if (dbName == null || HttpContext.Current.Request.Url.AbsolutePath == "/")
                return handlers.First(x => x.ComponentModel.Name == "centraldb");

            if (dbName["DatabaseName"] != null)
                return handlers.First(x => x.ComponentModel.Name == dbName["DatabaseName"].ToString());*/

            return handlers.First();//x => x.ComponentModel.Name == "centraldb");
        }
    }
}