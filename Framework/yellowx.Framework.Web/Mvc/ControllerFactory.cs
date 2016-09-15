using System;
using System.Web.Mvc;
using System.Web.Routing;
using yellowx.Framework.Extensions;
using yellowx.Framework.Dependency;

namespace yellowx.Framework.Web.Mvc
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private readonly IObjectFactory objectFactory;
        private readonly IActionInvoker actionInvoker;

        public ControllerFactory()
            :this(Configuration.Get<IObjectFactory>("objectFactory"), Configuration.Get<IActionInvoker>("actionInvoker"))
        {
            
        }
        public ControllerFactory(IObjectFactory objectFactory, IActionInvoker actionInvoker)
        {
            this.objectFactory = objectFactory;
            this.actionInvoker = actionInvoker;
        }

        public override void ReleaseController(IController controller)
        {
            if (objectFactory != null)
                objectFactory.Release(controller);
            else
                base.ReleaseController(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = objectFactory.IsNotNull(() => (IController)objectFactory.Resolve(controllerType), () => base.GetControllerInstance(requestContext, controllerType));
            if (controller != null && actionInvoker != null)
                ((Controller)controller).ActionInvoker = actionInvoker;

            return controller;
        }
    }
}
