using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Extensions;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Web
{
    public class Startup
    {
        #region Properties

        /// <summary>
        /// Get configuration root
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        #endregion

        #region Ctor

        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        #endregion

        /// <summary>
        /// Add services to the container
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //add options feature
            services.AddOptions();

            //add NopConfig configuration parameters
            var nopConfig = services.ConfigureStartupConfig<NopConfig>(Configuration.GetSection("Nop"));

            //add hosting configuration parameters
            services.ConfigureStartupConfig<HostingConfig>(Configuration.GetSection("Hosting"));

            //initialize engine
            var engine = services.InitializeNopEngine(nopConfig);

            //add MVC feature
            services.AddMvc();

            //add memory cache
            services.AddMemoryCache();

            //add Redis distributed cache
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = nopConfig.RedisCachingConnectionString;
            });

            //register mapper configurations
            services.AddAutoMapper();

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //return service provider provided by engine
            return engine.ServiceProvider;
        }

        /// <summary>
        /// Configure the HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in</param>
        /// <param name="routePublisher">Route publisher</param>
        public void Configure(IApplicationBuilder application, IHostingEnvironment environment, IRoutePublisher routePublisher)
        {
            //get detailed exceptions
            if (environment.IsDevelopment())
                application.UseDeveloperExceptionPage();
            else
                application.UseExceptionHandler("/Home/Error");

            //get access to HttpContext
            application.UseStaticHttpContext();

            //use MVC routing
            application.UseMvc(routeBuilder =>
            {
                //register all routes
                routePublisher.RegisterRoutes(routeBuilder);

                //default route
                routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
