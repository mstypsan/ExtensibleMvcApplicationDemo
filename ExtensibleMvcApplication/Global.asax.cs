using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ExtensibleMvcApplication.Infrastructure.Composition;
using ExtensibleMvcApplication.Infrastructure.Unity;
using Microsoft.Practices.Unity;

namespace ExtensibleMvcApplication
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name.
                "{controller}/{action}/{id}", // URL with parameters.
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults.
            );
        }

        private static void BootstrapContainer()
        {
            string extensionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");

            var discoverableControllerFactory = new DiscoverableControllerFactory(
                new CompositionContainer(
                    new DirectoryCatalog(extensionsPath)));

            var unityControllerFactory = new UnityControllerFactory(
                new UnityContainer() // No direct reference on the container outside this method.
                    .Install(Registrator.ForControllers,
                             Registrator.ForServices,
                             Registrator.ForEnterpriseLibrary),
                alternativeFactoryMethod: discoverableControllerFactory.CreateController);

            ControllerBuilder.Current.SetControllerFactory(unityControllerFactory);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BootstrapContainer();
        }
    }
}