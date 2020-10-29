using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nop.Core
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        Task<string> GetUrlReferrerAsync();

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        Task<string> GetCurrentIpAddressAsync();

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
        /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
        /// <returns>Page URL</returns>
        Task<string> GetThisPageUrlAsync(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        Task<bool> IsCurrentConnectionSecuredAsync();

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store host location</returns>
        Task<string> GetStoreHostAsync(bool useSsl);

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL; pass null to determine automatically</param>
        /// <returns>Store location</returns>
        Task<string> GetStoreLocationAsync(bool? useSsl = null);

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the CMS engine.
        /// </summary>
        /// <returns>True if the request targets a static resource file.</returns>
        Task<bool> IsStaticResourceAsync();

        /// <summary>
        /// Modify query string of the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="key">Query parameter key to add</param>
        /// <param name="values">Query parameter values to add</param>
        /// <returns>New URL with passed query parameter</returns>
        Task<string> ModifyQueryStringAsync(string url, string key, params string[] values);

        /// <summary>
        /// Remove query parameter from the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="key">Query parameter key to remove</param>
        /// <param name="value">Query parameter value to remove; pass null to remove all query parameters with the specified key</param>
        /// <returns>New URL without passed query parameter</returns>
        Task<string> RemoveQueryStringAsync(string url, string key, string value = null);

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        Task<T> QueryStringAsync<T>(string name);

        /// <summary>
        /// Restart application domain
        /// </summary>
        Task RestartAppDomainAsync();

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        bool IsPostBeingDone { get; set; }

        /// <summary>
        /// Gets current HTTP request protocol
        /// </summary>
        string CurrentRequestProtocol { get; }

        /// <summary>
        /// Gets whether the specified HTTP request URI references the local host.
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        Task<bool> IsLocalRequestAsync(HttpRequest req);

        /// <summary>
        /// Get the raw path and full query of request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        Task<string> GetRawUrlAsync(HttpRequest request);

        /// <summary>
        /// Gets whether the request is made with AJAX 
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Result</returns>
        Task<bool> IsAjaxRequestAsync(HttpRequest request);
    }
}
