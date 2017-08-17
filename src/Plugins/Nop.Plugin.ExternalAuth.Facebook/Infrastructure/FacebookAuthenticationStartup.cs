using Microsoft.AspNetCore.Authentication.Facebook;
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
            //TODO find the way to get settings here
            //var settings = EngineContext.Current.Resolve<FacebookExternalAuthSettings>();
            var settings = new FacebookExternalAuthSettings();
            if (string.IsNullOrEmpty(settings?.ClientKeyIdentifier) || string.IsNullOrEmpty(settings?.ClientSecret))
                return;

            services.AddAuthentication().AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
            {
                options.AppId = settings.ClientKeyIdentifier;
                options.AppSecret = settings.ClientSecret;
                options.SaveTokens = true;
            });
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
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
