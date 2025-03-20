using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using Roottech.Tracking.Common.Session;

namespace Roottech.Tracking.WebApi.Infrastructure.SessionManagement
{
    //Is up to you to:
    //-set the currentsessioncontextclass in nhibernate as follows:
    //		configuration.Properties[Environment.CurrentSessionContextClass]
    //      	= typeof (LazySessionContext).AssemblyQualifiedName;
    //-implement ISessionFactoryProvider or use Castle Typed Factories:
    //		container.Register(Component.For<ISessionFactoryProvider>().AsFactory());
    //-load the SessionFactoryProvider in the HttpApplication as follows:
    //		HttpContext.Current.Application[SessionFactoryProvider.Key]
    //				= your instance of ISessionFactoryProvider;
    //-inject ISessionFactory in Daos and use GetCurrentSessionContext()

    public interface ISessionFactoryProvider
    {
        IEnumerable<ISessionFactory> GetSessionFactories();
    }

    public class SessionFactoryProvider
    {
        public const string Key = "NHibernateSessionFactoryProvider";
    }

    public class NHibernateSessionModule : IHttpModule
    {
        private HttpApplication app;
		
        public void Init(HttpApplication context)
        {
            app = context;
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
            context.Error += ContextError;
        }

        private void ContextBeginRequest(object sender, EventArgs e)
        {
            var sfp = (ISessionFactoryProvider)app.Context.Application[SessionFactoryProvider.Key];
            foreach (var sf in sfp.GetSessionFactories())
            {
                var localFactory = sf;
                LazySessionContext.Bind(
                    new Lazy<ISession>(() => BeginSession(localFactory)), 
                    sf);
            }
        }

        private static ISession BeginSession(ISessionFactory sf)
        {
            var session = sf.OpenSession();
            session.BeginTransaction();
            return session;
        }

        private void ContextEndRequest(object sender, EventArgs e)
        {
            var sfp = (ISessionFactoryProvider)app.Context.Application[SessionFactoryProvider.Key];
            var sessionsToEnd = sfp.GetSessionFactories().Select(LazySessionContext.UnBind).Where(session => session != null);
            foreach (var session in sessionsToEnd)
            {
                EndSession(session);
            }
        }

        private void ContextError(object sender, EventArgs e)
        {
            var sfp = (ISessionFactoryProvider)app.Context.Application[SessionFactoryProvider.Key];
            var sessionstoAbort = sfp.GetSessionFactories().Select(LazySessionContext.UnBind).Where(session => session != null);
            foreach (var session in sessionstoAbort)
            {
                //session.Transaction.Commit(); //CRUD Not suggestted in the example //http://stackoverflow.com/questions/9583480/how-to-manage-transactions-with-ninject-and-nhibernate-with-isessionfactory-inje
                EndSession(session, true);
            }
        }

        private static void EndSession(ISession session, bool abort = false)
        {
            try
            {
                if (session.Transaction != null && session.Transaction.IsActive) //CRUD
                    if (abort)
                        session.Transaction.Rollback();
                    else //if (commitable)
                        session.Transaction.Commit();
            }
            catch (Exception ex)
            {
                session.Transaction.Rollback();
                throw new ApplicationException("Error committing database transaction. " + ex.Message, ex);
            }
            finally
            {
                session.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                app.BeginRequest -= ContextBeginRequest;
                app.EndRequest -= ContextEndRequest;
                app.Error -= ContextError;
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
}