using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Http;
using Nop.Core.Infrastructure;

namespace Nop.Core
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        #region Fields 

        private readonly HostingConfig _hostingConfig;

        #endregion

        #region Ctor

        public WebHelper(HostingConfig hostingConfig)
        {
            this._hostingConfig = hostingConfig;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns>True if available; otherwise false</returns>
        protected virtual bool IsRequestAvailable()
        {
            if (HttpContext.Current == null)
                return false;

            try
            {
                return HttpContext.Current.Request == null;
            }
            catch { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            //try get URL referred from header
            var referedHeader = HttpContext.Current.Request.Headers[HeaderNames.Referer];
            if (StringValues.IsNullOrEmpty(referedHeader))
                return string.Empty;

            return referedHeader.ToString();
        }

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            try
            {
                //first try to get IP address from the forwarded header
                if (HttpContext.Current.Request.Headers != null)
                {
                    //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer
                    var forwardedHttpHeaderKey = "X-FORWARDED-FOR";
                    if (!string.IsNullOrEmpty(_hostingConfig.ForwardedHttpHeader))
                    {
                        //but in some cases server use other HTTP header
                        //in these cases an administrator can specify a custom Forwarded HTTP header (e.g. CF-Connecting-IP, X-FORWARDED-PROTO, etc)
                        forwardedHttpHeaderKey = _hostingConfig.ForwardedHttpHeader;
                    }

                    var forwardedHeader = HttpContext.Current.Request.Headers[forwardedHttpHeaderKey];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        result = forwardedHeader.FirstOrDefault();
                }

                //if this header not exists try get connection remote IP address
                if (string.IsNullOrEmpty(result) && HttpContext.Current.Connection.RemoteIpAddress != null)
                    result = HttpContext.Current.Connection.RemoteIpAddress.ToString();
            }
            catch { return string.Empty; }

            //some of the validation
            if (result.Equals("::1", StringComparison.InvariantCultureIgnoreCase))
                result = "127.0.0.1";

            //remove port
            if (!string.IsNullOrEmpty(result))
                result = result.Split(':').FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page URL</returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            //whether connection is secured
            var useSsl = IsCurrentConnectionSecured();

            return GetThisPageUrl(includeQueryString, useSsl);
        }

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL</param>
        /// <returns>Page URL</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            //get the host considering using SSL
            var host = GetStoreHost(useSsl).TrimEnd('/');

            //get full URL with or without query string
            var url = string.Format("{0}{1}{2}", host, HttpContext.Current.Request.Path,
                includeQueryString ? HttpContext.Current.Request.QueryString.Value : string.Empty);

            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;

#if NET451
            //check whehter hosting uses a load balancer on their server
            //1. use HTTP_CLUSTER_HTTPS?
            if (_hostingConfig.UseHttpClusterHttps.HasValue && _hostingConfig.UseHttpClusterHttps.Value)
                return ServerVariables("HTTP_CLUSTER_HTTPS").Equals("on", StringComparison.InvariantCultureIgnoreCase);

            //2. use HTTP_X_FORWARDED_PROTO?
            if (_hostingConfig.UseHttpXForwardedProto.HasValue && _hostingConfig.UseHttpXForwardedProto.Value)
                return ServerVariables("HTTP_X_FORWARDED_PROTO").Equals("https", StringComparison.OrdinalIgnoreCase);

#endif
            return HttpContext.Current.Request.IsHttps;
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store host location</returns>
        public virtual string GetStoreHost(bool useSsl)
        {
            var result = string.Empty;

            //try to get host from the request HOST header
            var hostHeader = HttpContext.Current.Request.Headers[HeaderNames.Host];
            if (!StringValues.IsNullOrEmpty(hostHeader))
                result = "http://" + hostHeader.FirstOrDefault();

            //whether database is installed
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //get current store (do not inject IWorkContext via constructor because it'll cause circular references)
                var currentStore = EngineContext.Current.Resolve<IStoreContext>().CurrentStore;
                if (currentStore == null)
                    throw new Exception("Current store cannot be loaded");

                if (string.IsNullOrEmpty(result))
                {
                    //HOST header is not available, it is possible only when HttpContext is not available (for example, running in a schedule task)
                    //in this case use URL of a store entity configured in admin area
                    result = currentStore.Url;
                }

                if (useSsl)
                {
                    //if secure URL specified let's use this URL, otherwise a store owner wants it to be detected automatically
                    result = !string.IsNullOrWhiteSpace(currentStore.SecureUrl) ? currentStore.SecureUrl : result.Replace("http://", "https://");
                }
                else
                {
                    if (currentStore.SslEnabled && !string.IsNullOrWhiteSpace(currentStore.SecureUrl))
                    {
                        //SSL is enabled in this store and secure URL is specified, so a store owner don't want it to be detected automatically.
                        //in this case let's use the specified non-secure URL
                        result = currentStore.Url;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result) && useSsl)
                {
                    //use secure connection
                    result = result.Replace("http://", "https://");
                }
            }

            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <returns>Store location</returns>
        public virtual string GetStoreLocation()
        {
            //whether connection is secured
            var useSsl = IsCurrentConnectionSecured();

            return GetStoreLocation(useSsl);
        }

        /// <summary>
        /// Gets store location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store location</returns>
        public virtual string GetStoreLocation(bool useSsl)
        {
            //get store host
            var host = GetStoreHost(useSsl).TrimEnd('/');

            //add application path base if exists
            if (IsRequestAvailable())
                host += HttpContext.Current.Request.PathBase;

            if (!host.EndsWith("/"))
                host += "/";

            return host.ToLowerInvariant();
        }

        /// <summary>
        /// Modify query string of the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStrings">Query string parameters to add</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL with added passed query string</returns>
        public virtual string ModifyQueryString(string url, IDictionary<string, string[]> queryStrings, string anchor = null)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (queryStrings == null || !queryStrings.Any())
                return url;

            var uri = new Uri(url);

            //get all current query string key value pairs as a collection
            var queryCollection = QueryHelpers.ParseQuery(uri.Query);

            //add passed parameters to collection
            foreach (var queryParameter in queryStrings)
                queryCollection.Add(queryParameter.Key, new StringValues(queryParameter.Value));

            //get the page path from URL without QueryString
            var pathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            //get new URL with added passed query
            url = QueryHelpers.AddQueryString(pathWithoutQueryString,
                queryCollection.ToDictionary(query => query.Key, query => query.Value.ToString().ToLowerInvariant()));
            
            //add anchor if specified
            if (!string.IsNullOrEmpty(anchor))
                url = string.Format("{0}#{1}", url, anchor);

            return url;
        }

        /// <summary>
        /// Remove query string from the URL
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryNames">Query parameter names to remove</param>
        /// <returns>New URL without passed query string</returns>
        public virtual string RemoveQueryString(string url, IEnumerable<string> queryNames)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (queryNames == null || !queryNames.Any())
                return url;

            var uri = new Uri(url);

            //get all query string key value pairs as a collection
            var queryCollection = QueryHelpers.ParseQuery(uri.Query);

            //remove query strings by the names if exist
            foreach (var name in queryNames)
                queryCollection.Remove(name);

            //get the page path from URL without QueryString
            var pathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);
            if (!queryCollection.Any())
                return pathWithoutQueryString;

            return QueryHelpers.AddQueryString(pathWithoutQueryString, 
                queryCollection.ToDictionary(query => query.Key, query => query.Value.ToString().ToLowerInvariant()));
        }

        /// <summary>
        /// Gets query string value by name
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(HttpContext.Current.Request.Query[name]))
                return default(T);

            return CommonHelper.To<T>(HttpContext.Current.Request.Query[name].ToString());
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        public virtual void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "")
        {
#if NET451
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();

                TryWriteGlobalAsax();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    throw new NopException("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'web.config' file.");
                }
                success = TryWriteGlobalAsax();

                if (!success)
                {
                    throw new NopException("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (HttpContext.Current != null && makeRedirect)
            {
                if (String.IsNullOrEmpty(redirectUrl))
                    redirectUrl = GetThisPageUrl(true);
                HttpContext.Current.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
#endif
        }

        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
#if NET451
                var response = HttpContext.Current.Response;

                return response.IsRequestBeingRedirected;
#else 
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        public virtual bool IsPostBeingDone
        {
            get
            {
                if (HttpContext.Current.Items["nop.IsPOSTBeingDone"] == null)
                    return false;

                return Convert.ToBoolean(HttpContext.Current.Items["nop.IsPOSTBeingDone"]);
            }
            set
            {
                HttpContext.Current.Items["nop.IsPOSTBeingDone"] = value;
            }
        }

        #endregion
    }
}