using System;
using System.Linq;
using System.Net;
using Azure.Identity;
using Azure.Storage.Blobs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Core.Redis;
using Nop.Core.Security;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using StackExchange.Profiling.Storage;
using WebMarkupMin.AspNetCore5;
using WebMarkupMin.NUglify;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        /// <param name="webHostEnvironment">Hosting environment</param>
        /// <returns>Configured engine and app settings</returns>
        public static (IEngine, AppSettings) ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            //create default file provider
            CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment);

            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //add configuration parameters
            var appSettings = new AppSettings();
            configuration.Bind(appSettings);
            services.AddSingleton(appSettings);
            AppSettingsHelper.SaveAppSettings(appSettings);

            //initialize plugins
            var mvcCoreBuilder = services.AddMvcCore();
            mvcCoreBuilder.PartManager.InitializePlugins(appSettings);

            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, configuration);

            return (engine, appSettings);
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig AddConfig<TConfig>(this IServiceCollection services, IConfiguration configuration)
            where TConfig : class, IConfig, new()
        {
            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config.Name, config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AntiforgeryCookie}";

                //whether to allow the use of anti-forgery cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = DataSettingsManager.IsDatabaseInstalled() && EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.SslEnabled
                    ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;

                //whether to allow the use of session values from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = DataSettingsManager.IsDatabaseInstalled() && EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.SslEnabled
                    ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });
        }

        /// <summary>
        /// Adds services required for themes support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddThemes(this IServiceCollection services)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
            });
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopDataProtection(this IServiceCollection services)
        {
            //check whether to persist data protection in Redis
            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.RedisConfig.Enabled && appSettings.RedisConfig.StoreDataProtectionKeys)
            {
                //store keys in Redis
                services.AddDataProtection().PersistKeysToStackExchangeRedis(() =>
                {
                    //For some reason, data protection services are registered earlier. This configuration is called even before the request queue starts. 
                    //Service provider has not yet been built and we cannot get the required service. 
                    //So we create a new instance of RedisConnectionWrapper() bypassing the DI.
                    var redisConnectionWrapper = new RedisConnectionWrapper(appSettings);
                    return redisConnectionWrapper.GetDatabaseAsync(appSettings.RedisConfig.DatabaseId ?? (int)RedisDatabaseNumber.DataProtectionKeys).Result;

                }, NopDataProtectionDefaults.RedisDataProtectionKey);
            }
            else if (appSettings.AzureBlobConfig.Enabled && appSettings.AzureBlobConfig.StoreDataProtectionKeys)
            {
                var blobServiceClient = new BlobServiceClient(appSettings.AzureBlobConfig.ConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(appSettings.AzureBlobConfig.DataProtectionKeysContainerName);
                var blobClient = blobContainerClient.GetBlobClient(NopDataProtectionDefaults.AzureDataProtectionKeyFile);

                var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

                if (!appSettings.AzureBlobConfig.DataProtectionKeysEncryptWithVault)
                    return;

                var keyIdentifier = appSettings.AzureBlobConfig.DataProtectionKeysVaultId;
                var credentialOptions = new DefaultAzureCredentialOptions();
                var tokenCredential = new DefaultAzureCredential(credentialOptions);

                dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(NopDataProtectionDefaults.DataProtectionKeysPath);
                var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);

                //configure the data protection system to persist keys to the specified directory
                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopAuthentication(this IServiceCollection services)
        {
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = NopAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = NopAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = NopAuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(NopAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = NopAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = DataSettingsManager.IsDatabaseInstalled() && EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.SslEnabled
                    ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });

            //add external authentication
            authenticationBuilder.AddCookie(NopAuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = NopAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;

                //whether to allow the use of authentication cookies from SSL protected page on the other store pages which are not
                options.Cookie.SecurePolicy = DataSettingsManager.IsDatabaseInstalled() && EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.SslEnabled
                    ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });

            //register and configure external authentication plugins now
            var typeFinder = new WebAppTypeFinder();
            var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            var externalAuthInstances = externalAuthConfigurations
                .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

            foreach (var instance in externalAuthInstances)
                instance.Configure(authenticationBuilder);
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddControllersWithViews();

            mvcBuilder.AddRazorRuntimeCompilation();

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.CommonConfig.UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                mvcBuilder.AddSessionStateTempDataProvider();
            }
            else
            {
                //use cookie-based temp data provider
                mvcBuilder.AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.TempDataCookie}";

                    //whether to allow the use of cookies from SSL protected page on the other store pages which are not
                    options.Cookie.SecurePolicy = DataSettingsManager.IsDatabaseInstalled() && EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.SslEnabled
                        ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
                });
            }

            services.AddRazorPages();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom display metadata provider
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider()));

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {
                //register all available validators from Nop assemblies
                var assemblies = mvcBuilder.PartManager.ApplicationParts
                    .OfType<AssemblyPart>()
                    .Where(part => part.Name.StartsWith("Nop", StringComparison.InvariantCultureIgnoreCase))
                    .Select(part => part.Assembly);
                configuration.RegisterValidatorsFromAssemblies(assemblies);

                //implicit/automatic validation of child properties
                configuration.ImplicitlyValidateChildProperties = true;
            });

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            return mvcBuilder;
        }

        /// <summary>
        /// Register custom RedirectResultExecutor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopRedirectResultExecutor(this IServiceCollection services)
        {
            //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
            services.AddScoped<IActionResultExecutor<RedirectResult>, NopRedirectResultExecutor>();
        }

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopMiniProfiler(this IServiceCollection services)
        {
            //whether database is already installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.CommonConfig.MiniProfilerEnabled)
            {
                services.AddMiniProfiler(miniProfilerOptions =>
                {
                    //use memory cache provider for storing each result
                    ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(appSettings.CacheConfig.DefaultCacheTime);

                    //determine who can access the MiniProfiler results
                    miniProfilerOptions.ResultsAuthorize = request => EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.AccessProfiling).Result;
                });
            }
        }

        /// <summary>
        /// Add and configure WebMarkupMin service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopWebMarkupMin(this IServiceCollection services)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            services
                .AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisableMinification = !EngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
                    options.DisableCompression = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RenderEmptyTagsWithSpace = true;
                    settings.CollapseTagsWithoutContent = true;
                });
        }

        /// <summary>
        /// Add and configure default HTTP clients
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopHttpClients(this IServiceCollection services)
        {
            //default client
            services.AddHttpClient(NopHttpDefaults.DefaultHttpClient).WithProxy();

            //client to request current store
            services.AddHttpClient<StoreHttpClient>();

            //client to request nopCommerce official site
            services.AddHttpClient<NopHttpClient>().WithProxy();

            //client to request reCAPTCHA service
            services.AddHttpClient<CaptchaHttpClient>().WithProxy();
        }
    }
}