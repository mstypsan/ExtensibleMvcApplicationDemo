using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace ExtensibleMvcApplication.Infrastructure.Unity
{
    internal sealed class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly UnityContainer container;

        public UnityControllerFactory(UnityContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;

            if (controllerType == null)
            {
                throw new HttpException(404, String.Format("The controller for path '{0}' could not be found or it does not implement IController.", requestContext.HttpContext.Request.Path));
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(string.Format("Type requested is not a controller: {0}", controllerType.Name), "controllerType");
            }

            try
            {
                controller = container.Resolve(controllerType) as IController;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(String.Format("Error resolving controller {0}", controllerType.Name), e);
            }

            return controller;
        }
    }
}