using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExtensibleMvcApplication.Infrastructure.Composition
{
    internal sealed class ComposableControllerFactory : DefaultControllerFactory
    {
        private CompositionContainer container;
        private Func<RequestContext, string, IController> defaultFactoryMethod;

        public ComposableControllerFactory(string catalogPath, Func<RequestContext, string, IController> defaultFactoryMethod)
        {
            this.container = new CompositionContainer(
                new DirectoryCatalog(catalogPath));

            this.defaultFactoryMethod = defaultFactoryMethod;
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Lazy<IController> controller = this.container
                .GetExports<IController, IDictionary<string, object>>()
                .Where(c => c.Metadata.ContainsKey("controllerName")
                         && c.Metadata["controllerName"].ToString() == controllerName)
                .FirstOrDefault();

            if (controller != null)
            {
                return controller.Value;
            }

            return this.defaultFactoryMethod.Invoke(requestContext, controllerName);
        }
    }
}