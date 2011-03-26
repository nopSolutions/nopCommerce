using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using FluentValidation.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Installation;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Themes;

namespace Nop.Web
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

            routes.MapRoute("Product",
                            "Product/{productId}/{SeName}",
                            new { controller = "Catalog", action = "Product", SeName = UrlParameter.Optional});

            routes.MapRoute("Category",
                            "Category/{categoryId}/{SeName}",
                            new { controller = "Catalog", action = "Category", SeName = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Nop.Web.Controllers" }
            );
        }

        protected void Application_Start()
        {
            //subscrive to events
            EventBroker.Instance.InstallingDatabase += InstallDatabase;

            //initialize engine context
            EngineContext.Initialize(false);

            //set dependency resolver
            var dependencyResolver = new NopDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseNopModel), new NopModelBinder());

            //remove all view engines
            ViewEngines.Engines.Clear();
            //except the themeable razor view engine we use
            ViewEngines.Engines.Add(new ThemableRazorViewEngine());

            //Add some functionality on top of the deafult ModelMetadataProvider
            ModelMetadataProviders.Current = new NopMetadataProvider();

            //Registering some regular mvc stuf
            ViewEngines.Engines.Add(new ThemableRazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //For debugging
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider
                .AddImplicitRequiredAttributeForValueTypes = false;

            ModelValidatorProviders.Providers.Add(
                new FluentValidationModelValidatorProvider(new NopValidatorFactory()));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //TODO:Get "WorkingTheme" to persist throught the session (cookie?).
            var defaultTheme =
                EngineContext.Current.Resolve<IThemeProvider>().GetThemeConfigurations().Where(x => x.IsDefault).
                    FirstOrDefault();
            EngineContext.Current.Resolve<IWorkContext>().WorkingTheme = defaultTheme == null ? string.Empty : defaultTheme.ThemeName;
        }

        protected void InstallDatabase(object sender, EventArgs e)
        {
            EngineContext.Current.Resolve<IInstallationService>().InstallData();
        }

        protected void ContainerBuilding(object sender, NopEventArgs<ContainerBuilder> e)
        {
            e.Value.Register(ctx => RouteTable.Routes).SingleInstance();
            e.Value.Register(ctx => ModelBinders.Binders).SingleInstance();
            e.Value.Register(ctx => ViewEngines.Engines).SingleInstance();
        }
    }
}