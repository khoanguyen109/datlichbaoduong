using yellowx.Framework.Dependency;
using yellowx.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Reflection;

namespace yellowx.Framework.Web.Mvc.IoC
{
    /// <summary>
    /// A action invoker will invoke the action with specified controller context. 
    /// Implementing ControllerActionInvoker allows to inject IOC properties into controller.
    /// </summary>
    public class WindsorActionInvoker : ControllerActionInvoker
    {
        private readonly IObjectFactory _objectFactory;

        public WindsorActionInvoker(IObjectFactory objectFactory)
        {
            _objectFactory = objectFactory;
        }

        protected override ActionExecutedContext InvokeActionMethodWithFilters(ControllerContext controllerContext, IList<IActionFilter> filters, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            filters.ForEach(f => _objectFactory.InjectProperties(f));
            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }

        protected override AuthenticationContext InvokeAuthenticationFilters(ControllerContext controllerContext, IList<IAuthenticationFilter> filters, ActionDescriptor actionDescriptor)
        {
            filters.ForEach(f => _objectFactory.InjectProperties(f));
            return base.InvokeAuthenticationFilters(controllerContext, filters, actionDescriptor);
        }
    }

    public static class ObjectFactoryExtensions
    {
        public static void InjectProperties(this IObjectFactory objectFactory, object target)
        {
            var type = target.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && objectFactory.CanResolve(property.PropertyType))
                {
                    var value = objectFactory.Resolve(property.PropertyType);
                    try
                    {
                        property.SetValue(target, value, null);
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
                        throw new System.Exception(message, ex);
                    }
                }
            }
        }
    }
}
