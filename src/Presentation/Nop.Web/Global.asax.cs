using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Mvc;
using MvcMiniProfiler;
using MvcMiniProfiler.MVCHelpers;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.EmbeddedViews;
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
            
            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);
            
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Nop.Web.Controllers" }
            );
        }

        protected void Application_Start()
        {
            //initialize engine context
            EngineContext.Initialize(false, DataSettingsHelper.DatabaseIsInstalled());

            //set dependency resolver
            var dependencyResolver = new NopDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseNopModel), new NopModelBinder());

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemableRazorViewEngine());
            }

            //Add some functionality on top of the deafult ModelMetadataProvider
            ModelMetadataProviders.Current = new NopMetadataProvider();

            //Registering some regular mvc stuf
            //ViewEngines.Engines.Add(new ThemableRazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            if (DataSettingsHelper.DatabaseIsInstalled() &&
                EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            {
                GlobalFilters.Filters.Add(new ProfilingActionFilter());
            }
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //For debugging
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider
                .AddImplicitRequiredAttributeForValueTypes = false;

            ModelValidatorProviders.Providers.Add(
                new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            //register virtual path provider for embedded views
            var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnsureDatabaseIsInstalled();

            if (DataSettingsHelper.DatabaseIsInstalled() && 
                EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (DataSettingsHelper.DatabaseIsInstalled() &&
                EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            {
                //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
                MiniProfiler.Stop();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        { 
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            switch (custom)
            {
                case "WorkingLanguage":
                    return EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id.ToString();
                default:
                    return base.GetVaryByCustomString(context, custom);
            }
        }

        protected void EnsureDatabaseIsInstalled()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string installUrl = string.Format("{0}install", webHelper.GetStoreLocation());
            if (!webHelper.IsStaticResource(this.Request) &&
                !DataSettingsHelper.DatabaseIsInstalled() &&
                !webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Response.Redirect(installUrl);
            }
        }

        protected void SetWorkingCulture()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                if (!webHelper.IsStaticResource(this.Request))
                {
                    if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}admin", webHelper.GetStoreLocation()), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //admin area


                        //always set culture to 'en-US'
                        //we set culture of admin area to 'en-US' because current implementation of Telerik grid 
                        //doesn't work well in other cultures
                        //e.g., editing decimal value in russian culture
                        var culture = new CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                    }
                    else
                    {
                        //public store
                        var workContext = EngineContext.Current.Resolve<IWorkContext>();
                        if (workContext.CurrentCustomer != null && workContext.WorkingLanguage != null)
                        {
                            var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;
                        }
                    }
                }
            }
        }
    }
}