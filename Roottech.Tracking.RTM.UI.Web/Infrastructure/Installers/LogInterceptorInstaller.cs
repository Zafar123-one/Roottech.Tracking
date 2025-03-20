using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.Interceptors;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Installers
{
    public class LogInterceptorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ControllerLogInterceptor>());
        }
    }
}