using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using NHibernate;
using Owin;
using Roottech.Tracking.WebApi.Infrastructure;
using Roottech.Tracking.WebApi.Infrastructure.Extensions;
using Roottech.Tracking.WebApi.Infrastructure.Models;
using Roottech.Tracking.WebApi.Infrastructure.Provider;

[assembly: OwinStartup(typeof(Roottech.Tracking.WebApi.App_Start.Startup))]
namespace Roottech.Tracking.WebApi.App_Start
{
    public class Startup
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();
        public void Configuration(IAppBuilder app)
        {
            ConfigureIoC();

            app.CreatePerOwinContext(() => Container.Resolve<ISessionFactory>());
            app.CreatePerOwinContext(() => Container.Resolve<IClientMasterRepository>());
            app.CreatePerOwinContext(() => Container.Resolve<IAuthenticationRepository>());
            //app.CreatePerOwinContext(() => Container.Resolve<IKeyedRepository<int, ClientMaster>>());
            //Container.Resolve<IClientMasterRepository>()
            //    .ValidateClient("DOTNET", "9331B65E-12CD-4A6F-9F29-3D92C460CA32");
            ConfigureAuth(app);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            //this is very important line cross orgin source(CORS)it is used to enable cross-site HTTP requests  
            //For security reasons, browsers restrict cross-origin HTTP requests  
            app.UseCors(CorsOptions.AllowAll);

            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),//token expiration time  
                Provider = Container.Resolve<AuthorizationServerProvider>(), //new AuthorizationServerProvider()
                //Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                //For creating the refresh token and regenerate the new access token
                RefreshTokenProvider = Container.Resolve<RefreshTokenProvider>()
            };

            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            //https://stackoverflow.com/questions/41944879/when-using-an-owin-startup-class-how-do-i-dispose-of-my-castle-windsor-containe
            app.RegisterForDisposal(Container);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);//register the request            
        }

        private void ConfigureIoC()
        {
            //To be able to inject IEnumerable<T> ICollection<T> IList<T> T[] use this:
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            //Documentation http://docs.castleproject.org/Windsor.Resolvers.ashx

            //To support typed factories add this:
            Container.AddFacility<TypedFactoryFacility>();
            //Documentation http://docs.castleproject.org/Windsor.Typed-Factory-Facility.ashx

            Container.Install(FromAssembly.This());
            Container.Kernel.AddHandlerSelector(new SessionFactoryHandlerSelector());

            // register WebApi controllers
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorHttpControllerActivator(Container));
        }
    }
}