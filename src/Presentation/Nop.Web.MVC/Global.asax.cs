using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using AutofacContrib.CommonServiceLocator;
using FluentValidation.Attributes;
using FluentValidation.Mvc;
using Microsoft.Practices.ServiceLocation;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Security.Permissions;
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
            //set dependency resolver
            var dependencyResolver = new NopDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //other MVC stuff
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider
                .AddImplicitRequiredAttributeForValueTypes = false;

            ModelValidatorProviders.Providers.Add(
                new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

        }

        protected void RegisterServiceLocator()
        {
            var serviceLocator = new AutofacServiceLocator(EngineContext.Current.ContainerManager.Scope());
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //Service locator. We register it per request because ILifetimeScope could be changed per request
            //TODO uncomment to register ServiceLocator
            //RegisterServiceLocator();
        }
    }
}