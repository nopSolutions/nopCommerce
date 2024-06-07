using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework.Globalization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.WebOptimizer;
using QuestPDF.Drawing;
using WebMarkupMin.AspNetCore8;
using WebOptimizer;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Nop.Web.Framework.Infrastructure.Extensions;

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

    public static async Task StartEngineAsync(this IApplicationBuilder _)
    {
        var engine = EngineContext.Current;

        //further actions are performed only when the database is installed
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            //log application start
            await engine.Resolve<ILogger>().InformationAsync("Application started");

            //install and update plugins
            var pluginService = engine.Resolve<IPluginService>();
            await pluginService.InstallPluginsAsync();
            await pluginService.UpdatePluginsAsync();

            //update nopCommerce core and db
            var migrationManager = engine.Resolve<IMigrationManager>();
            var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
            migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
            assembly = Assembly.GetAssembly(typeof(IMigrationManager));
            migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

            //insert new ACL permission if exists
            var permissionService = engine.Resolve<IPermissionService>();
            await permissionService.InsertPermissionsAsync();

            var taskScheduler = engine.Resolve<ITaskScheduler>();
            await taskScheduler.InitializeAsync();
            await taskScheduler.StartSchedulerAsync();
        }
    }

    /// <summary>
    /// Add exception handling
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopExceptionHandler(this IApplicationBuilder application)
    {
        var appSettings = EngineContext.Current.Resolve<AppSettings>();
        var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
        var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
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
            handler.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception == null)
                    return;

                try
                {
                    //check whether database is installed
                    if (DataSettingsManager.IsDatabaseInstalled())
                    {
                        //get current customer
                        var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

                        //log error
                        await EngineContext.Current.Resolve<ILogger>().ErrorAsync(exception.Message, exception, currentCustomer);
                    }
                }
                finally
                {
                    //rethrow the exception to show the error page
                    ExceptionDispatchInfo.Throw(exception);
                }
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

                    if (DataSettingsManager.IsDatabaseInstalled())
                    {
                        var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

                        if (commonSettings.Log404Errors)
                        {
                            var logger = EngineContext.Current.Resolve<ILogger>();
                            var workContext = EngineContext.Current.Resolve<IWorkContext>();

                            await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
                                customer: await workContext.GetCurrentCustomerAsync());
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
            }
        });
    }

    /// <summary>
    /// Adds a special handler that checks for responses with the 400 status code (bad request)
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseBadRequestResult(this IApplicationBuilder application)
    {
        application.UseStatusCodePages(async context =>
        {
            //handle 404 (Bad request)
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                await logger.ErrorAsync("Error 400. Bad request", null, customer: await workContext.GetCurrentCustomerAsync());
            }
        });
    }

    /// <summary>
    /// Configure middleware for dynamically compressing HTTP responses
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopResponseCompression(this IApplicationBuilder application)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //whether to use compression (gzip by default)
        if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
            application.UseResponseCompression();
    }

    /// <summary>
    /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopWebOptimizer(this IApplicationBuilder application)
    {
        var appSettings = Singleton<AppSettings>.Instance;
        var woConfig = appSettings.Get<WebOptimizerConfig>();

        if (!woConfig.EnableCssBundling && !woConfig.EnableJavaScriptBundling)
            return;

        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

        application.UseWebOptimizer(webHostEnvironment,
        [
            new FileProviderOptions
            {
                RequestPath =  new PathString("/Plugins"),
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins"))
            },
            new FileProviderOptions
            {
                RequestPath =  new PathString("/Themes"),
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes"))
            }
        ]);
    }

    /// <summary>
    /// Configure static file serving
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopStaticFiles(this IApplicationBuilder application)
    {
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        var appSettings = EngineContext.Current.Resolve<AppSettings>();

        void staticFileResponse(StaticFileResponseContext context)
        {
            if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
                context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
        }

        //add handling if sitemaps 
        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopSeoDefaults.SitemapXmlDirectory)),
            RequestPath = new PathString($"/{NopSeoDefaults.SitemapXmlDirectory}"),
            OnPrepareResponse = context =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled() ||
                    !EngineContext.Current.Resolve<SitemapXmlSettings>().SitemapXmlEnabled)
                {
                    context.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Context.Response.ContentLength = 0;
                    context.Context.Response.Body = Stream.Null;
                }
            }
        });

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

        //exclude files in blacklist
        if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist))
        {
            var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

            foreach (var ext in appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist
                         .Split(';', ',')
                         .Select(e => e.Trim().ToLowerInvariant())
                         .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                         .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
            {
                fileExtensionContentTypeProvider.Mappings.Remove(ext);
            }

            staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
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
            ContentTypeProvider = provider,
            OnPrepareResponse = context =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled() ||
                    !EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermission.System.MANAGE_MAINTENANCE).Result)
                {
                    context.Context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Context.Response.ContentLength = 0;
                    context.Context.Response.Body = Stream.Null;
                }
            }
        });

        //add support for webmanifest files
        provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
            RequestPath = "/icons",
            ContentTypeProvider = provider
        });

        if (DataSettingsManager.IsDatabaseInstalled())
        {
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
                RequestPath = new PathString(NopRoxyFilemanDefaults.DefaultRootDirectory),
                OnPrepareResponse = staticFileResponse
            });
        }

        if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
        {
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
                RequestPath = new PathString("/.well-known"),
                ServeUnknownFileTypes = true,
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
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        application.UseMiddleware<AuthenticationMiddleware>();
    }

    /// <summary>
    /// Configure PDF
    /// </summary>
    public static void UseNopPdf(this IApplicationBuilder _)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        var fontPaths = fileProvider.EnumerateFiles(fileProvider.MapPath("~/App_Data/Pdf/"), "*.ttf") ?? Enumerable.Empty<string>();

        //write placeholder characters instead of unavailable glyphs for both debug/release configurations
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

        foreach (var fp in fontPaths)
        {
            FontManager.RegisterFont(File.OpenRead(fp));
        }
    }

    /// <summary>
    /// Configure the request localization feature
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopRequestLocalization(this IApplicationBuilder application)
    {
        application.UseRequestLocalization(options =>
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

            //prepare supported cultures
            var cultures = languageService
                .GetAllLanguages()
                .OrderBy(language => language.DisplayOrder)
                .Select(language => new CultureInfo(language.LanguageCulture))
                .ToList();
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault() ?? new CultureInfo(NopCommonDefaults.DefaultLanguageCulture));
            options.ApplyCurrentCultureToResponseHeaders = true;

            //configure culture providers
            options.AddInitialRequestCultureProvider(new NopSeoUrlCultureProvider());
            var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
            if (cookieRequestCultureProvider is not null)
                cookieRequestCultureProvider.CookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";
            if (!localizationSettings.AutomaticallyDetectLanguage)
            {
                var headerRequestCultureProvider = options
                    .RequestCultureProviders
                    .OfType<AcceptLanguageHeaderRequestCultureProvider>()
                    .FirstOrDefault();
                if (headerRequestCultureProvider is not null)
                    options.RequestCultureProviders.Remove(headerRequestCultureProvider);
            }
        });
    }

    /// <summary>
    /// Configure Endpoints routing
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopEndpoints(this IApplicationBuilder application)
    {
        //Execute the endpoint selected by the routing middleware
        application.UseEndpoints(endpoints =>
        {
            //register all routes
            EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
        });
    }

    /// <summary>
    /// Configure applying forwarded headers to their matching fields on the current request.
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopProxy(this IApplicationBuilder application)
    {
        var appSettings = EngineContext.Current.Resolve<AppSettings>();
        var hostingConfig = appSettings.Get<HostingConfig>();

        if (hostingConfig.UseProxy)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                ForwardLimit = 2
            };

            if (!string.IsNullOrEmpty(hostingConfig.ForwardedForHeaderName))
                options.ForwardedForHeaderName = hostingConfig.ForwardedForHeaderName;

            if (!string.IsNullOrEmpty(hostingConfig.ForwardedProtoHeaderName))
                options.ForwardedProtoHeaderName = hostingConfig.ForwardedProtoHeaderName;

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            if (!string.IsNullOrEmpty(hostingConfig.KnownProxies))
            {
                foreach (var strIp in hostingConfig.KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    if (IPAddress.TryParse(strIp, out var ip))
                        options.KnownProxies.Add(ip);
                }
            }

            if (!string.IsNullOrEmpty(hostingConfig.KnownNetworks))
            {
                foreach (var strIpNet in hostingConfig.KnownNetworks.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var ipNetParts = strIpNet.Split("/");
                    if (ipNetParts.Length == 2)
                    {
                        if (IPAddress.TryParse(ipNetParts[0], out var ip) && int.TryParse(ipNetParts[1], out var length))
                            options.KnownNetworks.Add(new IPNetwork(ip, length));
                    }
                }
            }

            if (options.KnownProxies.Count > 1 || options.KnownNetworks.Count > 1)
                options.ForwardLimit = null; //disable the limit, because KnownProxies is configured

            //configure forwarding
            application.UseForwardedHeaders(options);
        }
    }

    /// <summary>
    /// Configure WebMarkupMin
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopWebMarkupMin(this IApplicationBuilder application)
    {
        //check whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        application.UseWebMarkupMin();
    }
}