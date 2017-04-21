using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Extensions;
using Nop.Web.Framework.Extensions;

namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of appllication
    /// </summary>
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
            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        #endregion

        /// <summary>
        /// Add services to the container
        /// </summary>
        /// <param name="services">The contract for a collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //add and configure MVC feature
            services.AddNopMvc();

            //add MiniProfiler services
            services.AddMiniProfiler();

            //add and configure Nop engine
            var engine = services.AddNopEngine(Configuration);

            //return service provider provided by engine
            return engine.ServiceProvider;
        }

        /// <summary>
        /// Configure the HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder that provides the mechanisms to configure an application's request pipeline</param>
        /// <param name="environment">Provides information about the web hosting environment an application is running in</param>
        public void Configure(IApplicationBuilder application, IHostingEnvironment environment)
        {
            //get detailed exceptions
            if (environment.IsDevelopment())
                application.UseDeveloperExceptionPage();
            else
                application.UseExceptionHandler("/Home/Error");

            //get access to HttpContext
            application.UseStaticHttpContext();

            //add MiniProfiler
            application.UseMiniProfiler();

            //use MVC routing
            application.UseNopMvc();
        }
    }
}
