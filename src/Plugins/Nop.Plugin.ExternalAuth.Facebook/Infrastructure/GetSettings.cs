using Microsoft.AspNetCore.Authentication.Facebook;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure
{
    /// <summary>
    /// Registration of Facebook settings
    /// </summary>
    public static class GetSettings
    {
        /// <summary>
        /// Configure
        /// </summary>
        public static FacebookOptions Configure()
        {
            var settings = EngineContext.Current.Resolve<FacebookExternalAuthSettings>();
            return new FacebookOptions
            {
                AppId = settings.ClientKeyIdentifier,
                AppSecret = settings.ClientSecret,
                SaveTokens = true
            };
        }
    }
}
