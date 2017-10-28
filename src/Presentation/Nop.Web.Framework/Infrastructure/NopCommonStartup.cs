using System.IO;
using System.Linq;
using ImageResizer.Configuration;
using ImageResizer.Plugins.PrettyGifs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Compression;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring common features and middleware on application startup
    /// </summary>
    public class NopCommonStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //add memory cache
            services.AddMemoryCache();

            //add distributed memory cache
            services.AddDistributedMemoryCache();
                        
            //add HTTP sesion state feature
            services.AddHttpSession();

            //add anti-forgery
            services.AddAntiForgery();

            //add localization
            services.AddLocalization();

            //add theme support
            services.AddThemes();
            
            //add gif resizing support
            new PrettyGifs().Install(Config.Current);
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var nopConfig = EngineContext.Current.Resolve<NopConfig>();

            //compression
            if (nopConfig.UseResponseCompression)
            {
                //gzip by default
                application.UseResponseCompression();
                //workaround with "vary" header
                application.UseMiddleware<ResponseCompressionVaryWorkaroundMiddleware>();
            }

            //static files
            application.UseStaticFiles(new StaticFileOptions
            {
                //TODO duplicated code (below)
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, nopConfig.StaticFilesCacheControl);
                }
            });
            //themes
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Themes")),
                RequestPath = new PathString("/Themes"),
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, nopConfig.StaticFilesCacheControl);
                }
            });
            
            //plugins
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Plugins")),
                RequestPath = new PathString("/Plugins"),
                OnPrepareResponse = ctx =>
                {
                    if (!string.IsNullOrEmpty(nopConfig.StaticFilesCacheControl))
                        ctx.Context.Response.Headers.Append(HeaderNames.CacheControl,
                            nopConfig.StaticFilesCacheControl);
                }
            };
            //whether database is installed
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
                if (!string.IsNullOrEmpty(securitySettings.PluginStaticFileExtensionsBlacklist))
                {
                    var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

                    foreach (var ext in securitySettings.PluginStaticFileExtensionsBlacklist
                        .Split(';', ',')
                        .Select(e => e.Trim().ToLower())
                        .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                        .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                    {
                        fileExtensionContentTypeProvider.Mappings.Remove(ext);
                    }

                    staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
                }
            }
            application.UseStaticFiles(staticFileOptions);

            //add support for backups
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".bak"] = MimeTypes.ApplicationOctetStream;
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "db_backups")),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider
            });

            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //check whether database is installed
            application.UseInstallUrl();

            //use HTTP session
            application.UseSession();

            //use request localization
            application.UseRequestLocalization();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //common services should be loaded after error handlers
            get { return 100; }
        }
    }
}
