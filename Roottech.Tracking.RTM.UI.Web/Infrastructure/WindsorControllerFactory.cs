using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404,
                                        string.Format("The controller for path '{0}' could not be found.",
                                                      requestContext.HttpContext.Request.Path));
            }
            //var controller = kernel.Resolve(controllerType) as Controller;
            //if (controller != null) controller.ActionInvoker = kernel.Resolve<IActionInvoker>();

            return (IController)kernel.Resolve(controllerType);//controller; //
        }
    }

    /*
public class WindsorActionInvoker : ControllerActionInvoker
{
    readonly IWindsorContainer _container;

    public WindsorActionInvoker(IWindsorContainer container)
    {
        this._container = container;
    }

    protected override ActionExecutedContext InvokeActionMethodWithFilters(
            ControllerContext controllerContext,
            IList<IActionFilter> filters,
            ActionDescriptor actionDescriptor,
            IDictionary<string, object> parameters)
    {
        foreach (IActionFilter actionFilter in filters)
        {
            _container.Kernel.InjectProperties(actionFilter);
        }
        return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
    }
}

public static class WindsorExtension
{
    public static void InjectProperties(this IKernel kernel, object target)
    {
        var type = target.GetType();
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanWrite && kernel.HasComponent(property.PropertyType))
            {
                var value = kernel.Resolve(property.PropertyType);
                try
                {
                    property.SetValue(target, value, null);
                }
                catch (Exception ex)
                {
                    var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
                    throw new ComponentActivatorException(message, ex);
                }
            }
        }
    }
}*/
}