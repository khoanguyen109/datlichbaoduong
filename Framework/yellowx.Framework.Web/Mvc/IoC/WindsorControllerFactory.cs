using Castle.Windsor;
using yellowx.Framework.Dependency;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace yellowx.Framework.Web.Mvc
{
    /// <summary>
    /// Implement a Windsor controller factory.
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IObjectFactory _objectFactory;
        private readonly IActionInvoker _actionInvoker;

        public WindsorControllerFactory(IObjectFactory objectFactory)
            : this(objectFactory, null)
        {
        }

        public WindsorControllerFactory(IObjectFactory objectFactory, IActionInvoker actionInvoker)
        {
            _objectFactory = objectFactory;
            _actionInvoker = actionInvoker;
        }

        public override void ReleaseController(IController controller)
        {
            _objectFactory.Release(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = (Controller)_objectFactory.Resolve(controllerType);
            if (controller == null && _actionInvoker != null)
                controller.ActionInvoker = _actionInvoker;

            return controller;
        }
    }
}