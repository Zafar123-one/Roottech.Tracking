using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Dispatcher
{
    public class WindsorCompositionRoot : IHttpControllerActivator
    {
        private readonly IWindsorContainer _container;
        public WindsorCompositionRoot(IWindsorContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scope = _container.BeginScope(); //BeginScope();
            var controller = (IHttpController)_container.Resolve(controllerType);//, new { request = request });
            request.RegisterForDispose(new Release(() => _container.Release(controller),scope.Dispose));
            return controller;
        }
        /*
        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(_container.Kernel);
        }*/

        private class Release : IDisposable
        {
            private readonly Action _release;
            private readonly Action _scope;

            public Release(Action release, Action scope)
            {
                _release = release;
                _scope = scope;
            }

            public void Dispose()
            {
                _release();
                _scope();
            }
        }
        /*
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scope = _container.BeginLifetimeScope();
            var controller = (IHttpController) scope.Resolve(controllerType);
            request.RegisterForDispose(scope);
            return controller;
        }

//If you want to register dependencies that are different for every request (like Hyprlinkr's RouteLinker), you can do this when beginning the lifetime scope:

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scope = _container.BeginLifetimeScope(x => RegisterRequestDependantResources(x, request));
            var controller = (IHttpController) scope.Resolve(controllerType);
            request.RegisterForDispose(scope);
            return controller;
        }

        private static void RegisterRequestDependantResources(ContainerBuilder containerBuilder, HttpRequestMessage request)
        {
            containerBuilder.RegisterInstance(new RouteLinker(request));
            containerBuilder.RegisterInstance(new ResourceLinkVerifier(request.GetConfiguration()));
        }*/
    }
}