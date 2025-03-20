using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Roottech.Tracking.WebApi.App_Start;
using Roottech.Tracking.WebApi.Infrastructure;
using Roottech.Tracking.WebApi.Infrastructure.Provider;
using Component = Castle.MicroKernel.Registration.Component;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Bootstrapper), "Wire")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Bootstrapper), "DeWire")]

namespace Roottech.Tracking.WebApi.App_Start
{
    public static class Bootstrapper
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        public static void Wire()
        {
            ////To be able to inject IEnumerable<T> ICollection<T> IList<T> T[] use this:
            //Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            ////Documentation http://docs.castleproject.org/Windsor.Resolvers.ashx

            ////To support typed factories add this:
            //Container.AddFacility<TypedFactoryFacility>();
            ////Documentation http://docs.castleproject.org/Windsor.Typed-Factory-Facility.ashx

            ////OWIN
            ////Container.Register(Component.For<AccessCodeProvider>().LifestyleScoped<OwinWebRequestScopeAccessor>());
            ////Container.Register(Component.For<AuthorizationServerProvider>()
            ////    .PropertiesIgnore((m, p) => p.PropertyType == typeof(Func<OAuthMatchEndpointContext, Task>)));

            //Container.Install(FromAssembly.This());
            //Container.Kernel.AddHandlerSelector(new SessionFactoryHandlerSelector());


            ////var controllerFactory = new WindsorControllerFactory(Container.Kernel);
            ////ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            ////GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(Container));

            ////GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AreaHttpControllerSelector(GlobalConfiguration.Configuration));

            //// register WebApi controllers
            //GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorHttpControllerActivator(Container));
        }

        public static void DeWire()
        {
            //Container.Dispose();
        }

        private static void InitializeContainer()
        {
            //var oldProvider = FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider);
            //FilterProviders.Providers.Remove(oldProvider);

            //Container.Register(Component.For<IWindsorContainer>().Instance(this.Container));
            //Container.Install(new BootstrapInstaller());

            //registerCustom();

            //Container.Install(new WebWindsorInstaller());

            //var provider = new WindsorFilterAttributeFilterProvider(this.Container);
            //FilterProviders.Providers.Add(provider);

            //DependencyResolver.SetResolver(new WindsorDependencyResolver(ServiceFactory.Container));

            // register WebApi controllers
            //GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorHttpControllerActivator(ServiceFactory.Container));
        }
    }
}