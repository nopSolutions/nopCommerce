using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IHttpClientBuilder
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Configure proxy connection for HTTP client (if enabled)
    /// </summary>
    /// <param name="httpClientBuilder">A builder for configuring HttpClient</param>
    public static void WithProxy(this IHttpClientBuilder httpClientBuilder)
    {
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(provider =>
        {
            var handler = new HttpClientHandler();

            //whether proxy is enabled
            ProxySettings proxySettings;
            try
            {
                proxySettings = provider.GetService<ProxySettings>();
            }
            catch (Exception exception)
            {
                //IHttpClientFactory keeps a handler factory exception until the app restarts,
                //so connect directly and read the settings again when the handler is rebuilt
                provider.GetService<ILoggerFactory>()?.CreateLogger(typeof(HttpClientBuilderExtensions)).LogWarning(
                    exception, "Proxy settings could not be loaded; connecting directly until the handler is rebuilt");

                return handler;
            }

            if (!proxySettings?.Enabled ?? true)
                return handler;

            //configure proxy connection
            var webProxy = new WebProxy($"{proxySettings.Address}:{proxySettings.Port}", proxySettings.BypassOnLocal);
            if (!string.IsNullOrEmpty(proxySettings.Username) && !string.IsNullOrEmpty(proxySettings.Password))
            {
                webProxy.UseDefaultCredentials = false;
                webProxy.Credentials = new NetworkCredential
                {
                    UserName = proxySettings.Username,
                    Password = proxySettings.Password
                };
            }
            else
            {
                webProxy.UseDefaultCredentials = true;
                webProxy.Credentials = CredentialCache.DefaultCredentials;
            }

            //configure HTTP client handler
            handler.UseDefaultCredentials = webProxy.UseDefaultCredentials;
            handler.Proxy = webProxy;
            handler.PreAuthenticate = proxySettings.PreAuthenticate;

            return handler;
        });
    }
}