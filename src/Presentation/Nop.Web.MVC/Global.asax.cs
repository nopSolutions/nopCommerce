using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Nop.Core.Infrastructure;

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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Start()
        {
            //build container
            var nopStarter = new NopStarter();
            nopStarter.ContainerBuilding += new EventHandler<ContainerBuilderEventArgs>(nopStarter_ContainerBuilding);
            nopStarter.ContainerBuildingComplete += new EventHandler<ContainerBuilderEventArgs>(nopStarter_ContainerBuildingComplete);
            var container = nopStarter.BuildContainer();
            
            //execute startup tasks
            nopStarter.ExecuteStartUpTasks();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private void nopStarter_ContainerBuilding(object sender, ContainerBuilderEventArgs e)
        {
            //register controllers
            e.Builder.RegisterControllers(typeof(MvcApplication).Assembly);
            //register plugins controllers - TODO uncomment
            //e.Builder.RegisterControllers(PluginManager.ReferencedPlugins.ToArray());
        }

        private void nopStarter_ContainerBuildingComplete(object sender, ContainerBuilderEventArgs e)
        {
        }
    }
}