using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Interceptors
{
    public class ControllerLogInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public ControllerLogInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            switch (invocation.Method.Name)
            {
                case "OnActionExecuting":
                    OnActionExecuting(invocation);
                    break;
                case "OnActionExecuted":
                    OnActionExecuted(invocation);
                    break;
            }
            invocation.Proceed();
        }

        private void OnActionExecuted(IInvocation invocation)
        {
            var actionExecutedContext = invocation.Arguments[0] as ActionExecutedContext;
            _logger.Debug("Executed action: " + invocation.TargetType.Name + "." +
                          actionExecutedContext.ActionDescriptor.ActionName);
        }

        /// <summary>
        /// 	For redirect to client selection page in case no client has been selected.
        /// </summary>
        private void OnActionExecuting(IInvocation invocation)
        {
            //Great Briton Culture selected before exectuing any action.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

            //If there is no client selected redirect to client selection page except in case of home, admin, account pages.
            /*var controller = new[] { "AdminController", "AccountController", "HomeController" };
            if (!((IList<string>)controller).Contains(invocation.TargetType.Name))
                if (((Controller)invocation.InvocationTarget).Session["DatabaseName"] == null)
                    ((Controller)invocation.InvocationTarget).Response.Redirect(((Controller)invocation.InvocationTarget).Url.RouteUrl(new { controller = "Home", action = "Index", area = "" }));
            */
            var actionExecutingContext = invocation.Arguments[0] as ActionExecutingContext;
            _logger.Debug("Executing action: " + invocation.TargetType.Name + "." +
                          actionExecutingContext.ActionDescriptor.ActionName);
        }
    }
}