using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Plugins;
using Nop.Web.Framework.Globalization;
using Nop.Web.Framework.Mvc.Routing;
using WebMarkupMin.AspNetCore3;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        public static void StartEngine(this IApplicationBuilder application)
        {
            var engine = EngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                //initialize and start schedule tasks
                Services.Tasks.TaskManager.Instance.Initialize();
                Services.Tasks.TaskManager.Instance.Start();

                //log application start
                engine.Resolve<ILogger>().Information("Application started");

                var pluginService = engine.Resolve<IPluginService>();
                
                //install plugins
                pluginService.InstallPlugins();

                //update plugins
                pluginService.UpdatePlugins();
            }
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopExceptionHandler(this IApplicationBuilder application)
        {
            var nopConfig = EngineContext.Current.Resolve<NopConfig>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var useDetailedExceptionPage = nopConfig.DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Error/Error");
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;

                    try
                    {
                        //check whether database is installed
                        if (DataSettingsManager.DatabaseIsInstalled)
                        {
                            //get current customer
                            var currentCustomer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;

                            //log error
                            EngineContext.Current.Resolve<ILogger>().Error(exception.Message, exception, currentCustomer);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }

                    return Task.CompletedTask;
                });
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        //get original path and query
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        if (DataSettingsManager.DatabaseIsInstalled)
                        {
                            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

                            if (commonSettings.Log404Errors)
                            {
                                var logger = EngineContext.Current.Resolve<ILogger>();
                                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                                logger.Error($"Error 404. The requested page ({originalPath}) was not found",
                                    customer: workContext.CurrentCustomer);
                            }
                        }

                        try
                        {
                            //get new path
                            var pageNotFoundPath = "/page-not-found";
                            //re-execute request with new path
                            context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                        }
                    }

                    await Task.CompletedTask;
                }
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    logger.Error("Error 400. Bad request", null, customer: workContext.CurrentCustomer);
                }

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Configure middleware for dynamically compressing HTTP responses
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopResponseCompression(this IApplicationBuilder application)
        {
            //whether to use compression (gzip by default)
            if (DataSettingsManager.DatabaseIsInstalled && EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
                application.UseResponseCompression();
        }

        /// <summary>
        /// Configure static file serving
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopStaticFiles(this IApplicationBuilder application)
        {
            static void staticFileResponse(StaticFileResponseContext context)
            {
                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                var commonSettings = EngineContext.Current.Resolve<CommonSettings>();
                if (!string.IsNullOrEmpty(commonSettings.StaticFilesCacheControl))
                    context.Context.Response.Headers.Append(HeaderNames.CacheControl, commonSettings.StaticFilesCacheControl);
            }

            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            //common static files
            application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

            //themes static files
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes")),
                RequestPath = new PathString("/Themes"),
                OnPrepareResponse = staticFileResponse
            });

            //plugins static files
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins")),
                RequestPath = new PathString("/Plugins"),
                OnPrepareResponse = staticFileResponse
            };

            if (DataSettingsManager.DatabaseIsInstalled)
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
            var provider = new FileExtensionContentTypeProvider
            {
                Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
            };

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath)),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider
            });

            //add support for webmanifest files
            provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
                RequestPath = "/icons",
                ContentTypeProvider = provider
            });

            if (DataSettingsManager.DatabaseIsInstalled)
            {
                application.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new RoxyFilemanProvider(fileProvider.GetAbsolutePath(NopRoxyFilemanDefaults.DefaultRootDirectory.TrimStart('/').Split('/'))),
                    RequestPath = new PathString(NopRoxyFilemanDefaults.DefaultRootDirectory),
                    OnPrepareResponse = staticFileResponse
                });
            }
        }

        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Configure middleware checking whether database is installed
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// Adds the authentication middleware, which enables authentication capabilities.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            application.UseMiddleware<AuthenticationMiddleware>();
        }

        /// <summary>
        /// Configure the request localization feature
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopRequestLocalization(this IApplicationBuilder application)
        {
            application.UseRequestLocalization(options =>
            {
                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //prepare supported cultures
                var cultures = EngineContext.Current.Resolve<ILanguageService>().GetAllLanguages()
                    .OrderBy(language => language.DisplayOrder)
                    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
                options.SupportedCultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
            });
        }       

        /// <summary>
        /// Set current culture info
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseCulture(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            application.UseMiddleware<CultureMiddleware>();
        }

        /// <summary>
        /// Configure Endpoints routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopEndpoints(this IApplicationBuilder application)
        {
            //Add the EndpointRoutingMiddleware
            application.UseRouting();

            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });
        }

        /// <summary>
        /// Configure WebMarkupMin
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseNopWebMarkupMin(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            application.UseWebMarkupMin();
        }
    }
}
