using System.Collections.Generic;

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
        string GetUrlReferrer();

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page URL</returns>
        string GetThisPageUrl(bool includeQueryString);

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL</param>
        /// <returns>Page URL</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store host location</returns>
        string GetStoreHost(bool useSsl);

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <returns>Store location</returns>
        string GetStoreLocation();

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store location</returns>
        string GetStoreLocation(bool useSsl);

        /// <summary>
        /// Modify query string of the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStrings">Query string parameters to add</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL with added passed query string</returns>
        string ModifyQueryString(string url, IDictionary<string, string[]> queryStrings, string anchor = null);

        /// <summary>
        /// Remove query string from the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryNames">Query parameter names to remove</param>
        /// <returns>New URL without passed query string</returns>
        string RemoveQueryString(string url, IEnumerable<string> queryNames);

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");
        
        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        bool IsPostBeingDone { get; set; }
    }
}
