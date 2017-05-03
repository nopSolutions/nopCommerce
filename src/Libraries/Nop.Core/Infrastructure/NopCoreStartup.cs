using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.Extensions;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring core services and middleware on application startup
    /// </summary>
    public class NopCoreStartup : IStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            //add options feature
            services.AddOptions();

            //add NopConfig configuration parameters
            var nopConfig = services.ConfigureStartupConfig<NopConfig>(configuration.GetSection("Nop"));

            //add hosting configuration parameters
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));

            if (nopConfig.RedisCachingEnabled)
            {
                //add Redis distributed cache
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = nopConfig.RedisCachingConnectionString;
                });
            }
            else
                services.AddDistributedMemoryCache();

            //add memory cache
            services.AddMemoryCache();

            //add accessor to HttpContext
            services.AddHttpContextAccessor();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //get access to HttpContext
            application.UseStaticHttpContext();

            //exception handling
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            application.UseExceptionHandler(hostingEnvironment.IsDevelopment());

            //handle 404 Page Not Found errors
            application.UsePageNotFound();

            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //check whether database is installed
            application.UseInstallUrl();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation (the lower the better) 
        /// </summary>
        public int Order
        {
            //load core services first
            get { return 0; }
        }
    }
}
