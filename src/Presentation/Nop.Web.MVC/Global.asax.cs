using System;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Infrastructure;
using Nop.Services.Security.Permissions;
using Nop.Core.Infrastructure.AutoFac;
using Nop.Web.MVC.Infrastructure;

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
            DependencyResolver.SetResolver(
                new AutofacDependencyResolver((Core.Context.Current.ContainerManager.Container)));


            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ////register permissions
            ////TODO move to NopStarter after implementing Common Service Locator pattern
            //var permissionProviders = DependencyResolver.Current.GetService<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
            //foreach (var providerType in permissionProviders)
            //{
            //    dynamic provider = Activator.CreateInstance(providerType);
            //    var repo = Nop.Core.Context.Current.Resolve<IRepository<Nop.Core.Domain.Security.Permissions.PermissionRecord>>();
            //    DependencyResolver.Current.GetService<IPermissionService>().InstallPermissions(provider);
            //}
        }
    }
}