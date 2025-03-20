using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Common.Repository.Implemenations;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.WebApi.Infrastructure.Models;

namespace Roottech.Tracking.WebApi.Infrastructure.Installers
{
    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(IKeyedRepository<,>))
                .Forward(typeof(IRepository<>))
                .Forward(typeof(IKeyed<>))
                .ImplementedBy(typeof(Repository<,>)).LifestylePerWebRequest()
                , Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifestylePerWebRequest(),

            Classes.FromAssemblyNamed("Roottech.Tracking.WebApi")
                .Where(type => type.Name.EndsWith("Repository"))
                .WithService.DefaultInterfaces()
                .Configure(c => c.LifestylePerWebRequest()));
        }
    }
}