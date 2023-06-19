using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.GoogleAuth;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.GoogleAuth.Infrastructure
{
    /// <summary>
    /// Represents registrar of Facebook authentication service
    /// </summary>
    public class GoogleAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.CookieManager = new ChunkingCookieManager();
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            builder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                //set credentials
                var settings = EngineContext.Current.Resolve<GoogleExternalAuthSettings>();

                options.ClientId = string.IsNullOrEmpty(settings?.ClientKeyIdentifier) ? nameof(options.ClientId) : settings.ClientKeyIdentifier;
                options.ClientSecret = string.IsNullOrEmpty(settings?.ClientSecret) ? nameof(options.ClientSecret) : settings.ClientSecret;
                
                //store access and refresh tokens for the further usage
                options.SaveTokens = true;

                //set custom events handlers
                options.Events = new OAuthEvents
                {
                    //in case of error, redirect the user to the specified URL
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();

                        var errorUrl = context.Properties.GetString(GoogleAuthenticationDefaults.ErrorCallback);
                        context.Response.Redirect(errorUrl);

                        return Task.FromResult(0);
                    }
                };
            });
        }
    }
}