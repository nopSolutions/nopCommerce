using System;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Installation;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System.Linq;
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
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            //installer
            EventBroker.Instance.InstallingDatabase += InstallDatabase;

            //initialize engine context
            EngineContext.Initialize(false);
            //set dependency resolver
            var dependencyResolver = new NopDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseNopModel),new NopModelBinder());

            //other MVC stuff

            //var themeableRazorViewEngine = new ThemeableViewEngine
            //                                   {
            //                                       CurrentTheme = httpContext => httpContext.Session["theme"] as string ?? string.Empty
            //                                   };
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(themeableRazorViewEngine);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemableRazorViewEngine());
            ModelMetadataProviders.Current = new NopMetadataProvider();
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            //set localization service for telerik
            Telerik.Web.Mvc.Infrastructure.DI.Current.Register(
                () => EngineContext.Current.Resolve<Telerik.Web.Mvc.Infrastructure.ILocalizationServiceFactory>());

            DataAnnotationsModelValidatorProvider
                .AddImplicitRequiredAttributeForValueTypes = false;

            ModelValidatorProviders.Providers.Add(
                new FluentValidationModelValidatorProvider(new NopValidatorFactory()));
        }

        //protected void RegisterServiceLocator()
        //{
        //    var serviceLocator = new AutofacServiceLocator(EngineContext.Current.ContainerManager.Scope());
        //    ServiceLocator.SetLocatorProvider(() => serviceLocator);
        //}

        protected void InstallDatabase(object sender, EventArgs e)
        {
            EngineContext.Current.Resolve<IInstallationService>().InstallData();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //TODO:Get "WorkingTheme" to persist throught the session (cookie?).
            var defaultTheme =
                EngineContext.Current.Resolve<IThemeProvider>().GetThemeConfigurations().Where(x => x.IsDefault).
                    FirstOrDefault();
            EngineContext.Current.Resolve<IWorkContext>().WorkingTheme = defaultTheme == null ? string.Empty : defaultTheme.ThemeName;

            //Service locator. We register it per request because ILifetimeScope could be changed per request
            //TODO uncomment to register ServiceLocator
            //RegisterServiceLocator();
        }
    }
}