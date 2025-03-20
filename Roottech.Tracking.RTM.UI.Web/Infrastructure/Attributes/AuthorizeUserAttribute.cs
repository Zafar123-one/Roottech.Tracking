using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NHibernate.Criterion;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMAM.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute//, IAuthorizationFilter
	{
        public AppType AccessPage { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized) return false;

            var repositoryDashboard =
                DependencyResolver.Current.GetService(typeof (IKeyedRepository<string, Dashboard>)) as
                    IKeyedRepository<string, Dashboard>;
            IList<AppType> objectNames = repositoryDashboard.QueryOver()
                .Select(Projections.Conditional(
                    Restrictions.Eq(Projections.Property<Dashboard>(x => x.ObjectName), "StationeryDashboard.aspx"), Projections.Constant(AppType.Stationary), Projections.Constant(AppType.Mobile)))
                .Where(x => x.Orgcode == Convert.ToDouble(SystemLibrary.GetCookies("UserOrgCode"))
                    && x.ObjectName.IsIn(new[] { "StationeryDashboard.aspx", "VehiclesDashboard.aspx" }))
                 .List<AppType>();

            return objectNames.Contains(AccessPage);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult("Default",
                new RouteValueDictionary(
                    new
                    {
                        controller = "Error",
                        action = "Unauthorised"
                    })
                );
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
            //AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //Write your code here to perform authorization
            //return true;
        }
    }

	/*public class ReAutorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			UserAuth userAuth = AuthModule.GetUserAuth(System.Web.HttpContext.Current);
			if (userAuth != null && !userAuth.IsAnonymous)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			var context = System.Web.HttpContext.Current;
			System.Web.HttpContext.Current.Session["returnUrl"] = context.Request.Url.AbsoluteUri;
			filterContext.Result = new RedirectToRouteResult(
			new RouteValueDictionary{
{ "action", "Login" },
{ "controller", "Home" },
			});
		}
	}*/
}