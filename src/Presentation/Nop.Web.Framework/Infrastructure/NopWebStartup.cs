using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Extensions;
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
            //add theme support
            services.AddThemes();

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

            //static files
            application.UseStaticFiles();
            //add support for backups
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".bak"] = MimeTypes.ApplicationOctetStream;
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "db_backups")),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider
            });
            //TODO temporary
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Themes")),
                RequestPath = new PathString("/Themes")
            });

            //set request culture
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //TODO move "DatabaseIsInstalled" validation to CultureMiddleware (an exception is thrown now for some reasons when on the installation page)
                application.UseCulture();
            }

            //use HTTP session
            application.UseSession();

            //handle 404 errors
            application.UsePageNotFound();

            //check whether database is installed
            application.UseInstallUrl();

            //MVC routing
            application.UseNopMvc();

            //add MiniProfiler
            application.UseMiniProfiler();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //web services should be loaded last
            get { return 1000; }
        }
    }
}
