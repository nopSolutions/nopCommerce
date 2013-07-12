using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using FluentValidation.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.EmbeddedViews;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Themes;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;

namespace Nop.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //do not register HandleErrorAttribute. use classic error handling mode
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
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
            //we use our own mobile devices support (".Mobile" is reserved). that's why we disable it.
            var mobileDisplayMode = DisplayModeProvider.Instance.Modes
                .FirstOrDefault(x => x.DisplayModeId == DisplayModeProvider.MobileDisplayModeId);
            if (mobileDisplayMode != null)
                DisplayModeProvider.Instance.Modes.Remove(mobileDisplayMode);

            //initialize engine context
            EngineContext.Initialize(false);

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            //set dependency resolver
            var dependencyResolver = new NopDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseNopModel), new NopModelBinder());

            if (databaseInstalled)
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new NopMetadataProvider();

            //Registering some regular mvc stuff
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            //StackExchange profiler
            if (CanPerformProfilingAction())
            {
                GlobalFilters.Filters.Add(new ProfilingActionFilter());
            }
            
            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            //register virtual path provider for embedded views
            var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            //start scheduled tasks
            if (databaseInstalled)
            {
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //keep alive page requested (we ignore it to prevent creating a guest customer records)
            string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetStoreLocation());
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;

            EnsureDatabaseIsInstalled();

            if (CanPerformProfilingAction())
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            if (CanPerformProfilingAction())
            {
                //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
                MiniProfiler.Stop();
            }

            //dispose registered resources
            //we do not register AutofacRequestLifetimeHttpModule as IHttpModule 
            //because it disposes resources before this Application_EndRequest method is called
            //and in this case the code above will throw an exception
            //UPDATE: we cannot do it. For more info see the following forum topic - http://www.nopcommerce.com/boards/t/22456/30-changeset-3db3868edcf2-loaderlock-was-detected.aspx
            //AutofacRequestLifetimeHttpModule.ContextEndRequest(sender, e);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        { 
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            //log error
            LogException(exception);

            //process 404 HTTP errors
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                if (!webHelper.IsStaticResource(this.Request))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;

                    // Call target Controller and pass the routeData.
                    IController errorController = EngineContext.Current.Resolve<Nop.Web.Controllers.CommonController>();

                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Common");
                    routeData.Values.Add("action", "PageNotFound");

                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
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
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //keep alive page requested (we ignore it to prevent creating a guest customer records)
            string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetStoreLocation());
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;


            if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}admin", webHelper.GetStoreLocation()),
                StringComparison.InvariantCultureIgnoreCase))
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

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;
            
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore 404 HTTP errors
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 &&
                !EngineContext.Current.Resolve<CommonSettings>().Log404Errors)
                return;

            try
            {
                //log
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentCustomer);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }

        protected bool CanPerformProfilingAction()
        {
            //will not run in medium trust
            if (CommonHelper.GetTrustLevel() < AspNetHostingPermissionLevel.High)
                return false;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return false;

            return EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore;
        }
    }
}