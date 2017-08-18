using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.DependencyInjection;
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
            //ignore external authentication now (validation exception is thrown)
            //https://github.com/nopSolutions/nopCommerce/issues/2497
            //builder.AddFacebook(FacebookDefaults.AuthenticationScheme, x => GetSettings.Configure());
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
