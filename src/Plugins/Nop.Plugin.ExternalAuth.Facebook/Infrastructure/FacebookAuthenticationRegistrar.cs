using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure
{
    /// <summary>
    /// Registration of Facebook authentication service (plugin)
    /// </summary>
    public class FacebookAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
            {
                var settings = EngineContext.Current.Resolve<FacebookExternalAuthSettings>();

                //no empty values allowed. otherwise, an exception could be thrown on application startup
                options.AppId = !String.IsNullOrWhiteSpace(settings.ClientKeyIdentifier) ? settings.ClientKeyIdentifier : "123";
                options.AppSecret = !String.IsNullOrWhiteSpace(settings.ClientSecret) ? settings.ClientSecret : "123";
                options.SaveTokens = true;
            });
        }

        /// <summary>
        /// Gets order of this registrar implementation
        /// </summary>
        public int Order
        {
            get { return 501; }
        }
    }
}
