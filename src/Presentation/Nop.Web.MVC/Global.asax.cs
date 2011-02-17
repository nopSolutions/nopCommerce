using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Security.Permissions;

namespace Nop.Web.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Catalog", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            //set dependency resolver
            var dependencyResolver = new AutofacDependencyResolver(Core.Context.Current.ContainerManager.Container);
            Core.Context.Current.ContainerManager.DependencyResolver = dependencyResolver;
            DependencyResolver.SetResolver(dependencyResolver);

            //other MVC stuff
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void RegisterDefaultPermissions()
        {
            //register permissions
            var permissionProviders = DependencyResolver.Current.GetService<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                var repo = DependencyResolver.Current.GetService<IRepository<Nop.Core.Domain.Security.Permissions.PermissionRecord>>();
                DependencyResolver.Current.GetService<IPermissionService>().InstallPermissions(provider);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //UNDONE it should be run only once on application startup (but application instance is not available yet in AutofacDependencyResolver)
            RegisterDefaultPermissions();

        }
    }
}