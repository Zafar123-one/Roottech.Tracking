using System;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Owin.Security.OAuth;
using Roottech.Tracking.WebApi.Infrastructure.Provider;
using Component = Castle.MicroKernel.Registration.Component;

namespace Roottech.Tracking.WebApi.Infrastructure.Installers
{
    public class OwinInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                //OWIN
                //https://stackoverflow.com/questions/31679798/owin-application-configuration-with-castle-typedfactory
                Component.For<AuthorizationServerProvider>()
                    .PropertiesIgnore((m, p) =>
                        {
                            return p.PropertyType == typeof(Func<OAuthMatchEndpointContext, Task>) ||
                                   p.PropertyType == typeof(Func<OAuthValidateTokenRequestContext, Task>) ||
                                   //p.PropertyType == typeof(Func<OAuthTokenEndpointContext, Task>) ||
                                   p.PropertyType == typeof(Func<OAuthTokenEndpointResponseContext, Task>);
                        }
                    ),
                Component.For<RefreshTokenProvider>()
            );
        }
    }
}