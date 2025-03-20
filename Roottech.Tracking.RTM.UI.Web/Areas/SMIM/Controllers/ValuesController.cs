using System.Collections.Generic;
using System.Web.Http;
using NHibernate;
using Roottech.Tracking.SMIM.Entities;

namespace Roottech.Tracking.FMS.UI.Web.Areas.SMIM.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly ISession _session;

        public ValuesController(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public IList<Login> Get(string userId, string password)
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return _session.GetNamedQuery("GetLogin")
                .SetString("Id", userId)
                .SetString("Password", password)//"86001C079F96D0546CF25928E85DD5D442A01560")
                .SetString("UserIP", ip) //"192.168.2.15")
                .List<Login>();
        }
    }
}
