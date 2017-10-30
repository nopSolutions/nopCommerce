using System;
using System.IO;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Web.Framework.FluentValidation;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Themes;

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
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Configured service provider</returns>
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //add NopConfig configuration parameters
            services.ConfigureStartupConfig<NopConfig>(configuration.GetSection("Nop"));
            //add hosting configuration parameters
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));
            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //create, initialize and configure the engine
            var engine = EngineContext.Create();
            engine.Initialize(services);
            var serviceProvider = engine.ConfigureServices(services, configuration);
            
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //implement schedule tasks
                //database is already installed, so start scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();

                //log application start
                EngineContext.Current.Resolve<ILogger>().Information("Application started", null, null);
            }

            return serviceProvider;
        }

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

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
                options.Cookie.Name = ".Nop.Antiforgery";
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
                options.Cookie.Name = ".Nop.Session";
                options.Cookie.HttpOnly = true;
            });
        }

        /// <summary>
        /// Adds services required for themes support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddThemes(this IServiceCollection services)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
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
            var nopConfig = services.BuildServiceProvider().GetRequiredService<NopConfig>();
            if (nopConfig.RedisCachingEnabled && nopConfig.PersistDataProtectionKeysToRedis)
            {
                //store keys in Redis
                services.AddDataProtection().PersistKeysToRedis(
                    () =>
                    {
                        var redisConnectionWrapper = EngineContext.Current.Resolve<IRedisConnectionWrapper>();
                        return redisConnectionWrapper.GetDatabase();
                    }, RedisConfiguration.DataProtectionKeysName);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.MapPath("~/App_Data/DataProtectionKeys");
                var dataProtectionKeysFolder = new DirectoryInfo(dataProtectionKeysPath);

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
                options.DefaultChallengeScheme = NopCookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = NopCookieAuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(NopCookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = NopCookieAuthenticationDefaults.CookiePrefix + NopCookieAuthenticationDefaults.AuthenticationScheme;
                options.Cookie.HttpOnly = true;
                options.LoginPath = NopCookieAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = NopCookieAuthenticationDefaults.AccessDeniedPath;
            });

            //add external authentication
            authenticationBuilder.AddCookie(NopCookieAuthenticationDefaults.ExternalAuthenticationScheme, options =>
             {
                 options.Cookie.Name = NopCookieAuthenticationDefaults.CookiePrefix + NopCookieAuthenticationDefaults.ExternalAuthenticationScheme;
                 options.Cookie.HttpOnly = true;
                 options.LoginPath = NopCookieAuthenticationDefaults.LoginPath;
                 options.AccessDeniedPath = NopCookieAuthenticationDefaults.AccessDeniedPath;
             });

            //register and configure external authentication plugins now
            var typeFinder = new WebAppTypeFinder();
            var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            var externalAuthInstances = externalAuthConfigurations
                .Where(x => PluginManager.FindPlugin(x)?.Installed ?? true) //ignore not installed plugins
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
            var mvcBuilder = services.AddMvc();

            //use session temp data provider
            mvcBuilder.AddSessionStateTempDataProvider();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom display metadata provider
            mvcBuilder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider()));

            //add custom model binder provider (to the top of the provider list)
            mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new NopModelBinderProvider()));

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration => configuration.ValidatorFactoryType = typeof(NopValidatorFactory));

            return mvcBuilder;
        }
    }
}
