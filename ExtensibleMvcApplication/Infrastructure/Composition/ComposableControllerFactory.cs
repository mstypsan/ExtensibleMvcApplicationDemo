﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExtensibleMvcApplication.Infrastructure.Composition
{
    internal sealed class ComposableControllerFactory : DefaultControllerFactory
    {
        private CompositionContainer compositionContainer;
        private Func<RequestContext, string, IController> defaultFactoryMethod;

        public ComposableControllerFactory(
            CompositionContainer compositionContainer, 
            Func<RequestContext, string, IController> defaultFactoryMethod)
        {
            this.compositionContainer = compositionContainer;
            this.defaultFactoryMethod = defaultFactoryMethod;
        }

        /// <summary>
        /// Creates the specified controller by using the specified request context.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP
        /// context and route data.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>
        /// The controller.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/>
        /// parameter is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentException">The <paramref name="controllerName"/>
        /// parameter is null or empty.</exception>
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Lazy<IController> controller = this.compositionContainer
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

        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public override void ReleaseController(IController controller)
        {
            IDisposable component = controller as IDisposable;

            if (component != null)
            {
                component.Dispose();
            }
        }
    }
}