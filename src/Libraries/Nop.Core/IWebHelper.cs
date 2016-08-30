using System.Web;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString);

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        bool IsCurrentConnectionSecured();
        
        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        string ServerVariables(string name);

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
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
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store location</returns>
        string GetStoreLocation(bool useSsl);

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        bool IsStaticResource(HttpRequest request);        

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New url</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// Remove query string from url
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New url</returns>
        string RemoveQueryString(string url, string queryString);
        
        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
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
