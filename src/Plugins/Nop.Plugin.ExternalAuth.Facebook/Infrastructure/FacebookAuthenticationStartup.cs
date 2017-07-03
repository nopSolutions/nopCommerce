using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring Facebook authentication middleware on application startup
    /// </summary>
    public class FacebookAuthenticationStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var settings = EngineContext.Current.Resolve<FacebookExternalAuthSettings>();
            if (string.IsNullOrEmpty(settings?.ClientKeyIdentifier) || string.IsNullOrEmpty(settings?.ClientSecret))
                return;

            //add Facebook middleware
            application.UseFacebookAuthentication(new FacebookOptions
            {
                AppId = settings.ClientKeyIdentifier,
                AppSecret = settings.ClientSecret,
                SaveTokens = true
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            get { return 501; }
        }
    }
}
