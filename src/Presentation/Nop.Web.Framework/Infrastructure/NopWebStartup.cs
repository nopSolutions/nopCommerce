using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring web services and middleware on application startup
    /// </summary>
    public class NopWebStartup : IStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //add HTTP sesion state feature
            services.AddHttpSession();

            //add localization
            services.AddLocalization();

            //add and configure MVC feature
            services.AddNopMvc();

            //add MiniProfiler services
            services.AddMiniProfiler();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use request localization
            application.UseRequestLocalization();

            //enable cookie authentication
            application.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "NopCookie",
                CookieHttpOnly = true
            });

            //set request culture
            application.UseCulture();

            //use HTTP session
            application.UseSession();

            //static files
            application.UseStaticFiles();
            //TODO temporary
            application.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
                RequestPath = new PathString("/Content")
            });
            application.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Scripts")),
                RequestPath = new PathString("/Scripts")
            });
            application.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Themes")),
                RequestPath = new PathString("/Themes")
            });

            //MVC routing
            application.UseNopMvc();

            //add MiniProfiler
            application.UseMiniProfiler();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation (the lower the better) 
        /// </summary>
        public int Order
        {
            //web services should be loaded last
            get { return 1000; }
        }
    }
}
