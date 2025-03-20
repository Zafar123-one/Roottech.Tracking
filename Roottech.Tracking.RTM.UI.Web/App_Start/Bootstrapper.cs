using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NHibernate;
using Roottech.Tracking.RTM.UI.Web.App_Start;
using Roottech.Tracking.RTM.UI.Web.Infrastructure;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Attributes;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher;

[assembly: WebActivator.PostApplicationStartMethod (typeof(Bootstrapper), "Wire")]
[assembly: WebActivator.ApplicationShutdownMethod (typeof(Bootstrapper), "DeWire")]

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public static class Bootstrapper
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        public static void Wire()
        {
            //To be able to inject IEnumerable<T> ICollection<T> IList<T> T[] use this:
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            //Documentation http://docs.castleproject.org/Windsor.Resolvers.ashx
            
            Container.Install(FromAssembly.This());
            Container.Kernel.AddHandlerSelector(new SessionFactoryHandlerSelector());
			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Container.Kernel));
            DependencyResolver.SetResolver(new WindsorDependencyResolver(Container.Kernel));

            //GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(Container.Kernel);
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(Container));

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AreaHttpControllerSelector(GlobalConfiguration.Configuration));
            /*
            var filterProviders = GlobalConfiguration.Configuration.Services.GetFilterProviders();
            Container.Register(
               Component.For<IEnumerable<IFilterProvider>>().Instance(filterProviders),
               Component.For<IFilterProvider>().ImplementedBy<WindsorFilterProvider>()
            );

            GlobalConfiguration.Configuration.Filters.Add(new TransactionAttribute(Container.Resolve<ISessionFactory>()));
            */
        }

        public static void DeWire()
        {
            /*foreach (var sessionFactory in container.ResolveAll<ISessionFactory>())
            {
                SqlDependency.Stop(sessionFactory.GetCurrentSession().Connection.ConnectionString);
            } */
            Container.Dispose();
        }
    }
}