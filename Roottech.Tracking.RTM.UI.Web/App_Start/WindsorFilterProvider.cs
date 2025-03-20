using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public class WindsorFilterProvider : IFilterProvider
    {
        private readonly IWindsorContainer _container;
        private readonly IEnumerable<IFilterProvider> _filterProviders;

        public WindsorFilterProvider(IWindsorContainer container, IEnumerable<IFilterProvider> filterProviders)
        {
            _container = container;
            _filterProviders = filterProviders;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var filters = _filterProviders.SelectMany(fp => fp.GetFilters(configuration, actionDescriptor)).ToList();
            foreach (var filter in filters) _container.Register(Component.For<IFilter>().Instance(filter.Instance));
            return filters;
        }
    }
}