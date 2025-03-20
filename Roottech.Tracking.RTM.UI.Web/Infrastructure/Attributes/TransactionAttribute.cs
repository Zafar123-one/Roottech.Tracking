using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using NHibernate;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Attributes
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        private readonly ISession _session;

        public TransactionAttribute(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            if (actionContext.ActionDescriptor.GetCustomAttributes<MyActionFilterAttribute>().Any())
            {
                _session.BeginTransaction(IsolationLevel.ReadCommitted);
                // The action is decorated with the marker attribute => 
                // do something with _foo
            }
            DependencyResolver.Current.GetService<ISession>().BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            ITransaction currentTransaction = DependencyResolver.Current.GetService<ISession>().Transaction;
            try
            {
                if (currentTransaction.IsActive) if (actionExecutedContext.Exception != null) currentTransaction.Rollback(); else currentTransaction.Commit();
            }
            finally
            {
                currentTransaction.Dispose();
            }
        }
    }

    public class MyActionFilterAttribute : Attribute
    {

    }
}