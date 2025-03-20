using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Roottech.Tracking.Library.Models;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Installers
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //To support typed factories add this:
            container.AddFacility<TypedFactoryFacility>();
            //Documentation http://docs.castleproject.org/Windsor.Typed-Factory-Facility.ashx`
            container.Register(Component.For<IModelFactory>().AsFactory());
        }
    }
}