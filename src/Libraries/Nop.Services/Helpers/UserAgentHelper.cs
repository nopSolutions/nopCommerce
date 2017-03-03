using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Services.Helpers
{
    /// <summary>
    /// User agent helper
    /// </summary>
    public partial class UserAgentHelper : IUserAgentHelper
    {
        private readonly NopConfig _config;
        private readonly HttpContextBase _httpContext;
        private readonly BrowscapXmlHelper _browscapXmlHelper;
        private ConcurrentDictionary<string, bool> _foundSearchEnginesCache;

        public const string IS_SEARCH_ENGINE_REQUEST_COOKIE_KEY = "ise";
        

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="httpContext">HTTP context</param>
        public UserAgentHelper(NopConfig config, HttpContextBase httpContext, BrowscapXmlHelper browscapXmlHelper)
        {
            this._config = config;
            this._httpContext = httpContext;
            this._browscapXmlHelper = browscapXmlHelper;
            this._foundSearchEnginesCache = new ConcurrentDictionary<string, bool>();
        }

       
        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine()
        {
            if (_httpContext == null)
                return false;

            //we put required logic in try-catch block
            //more info: http://www.nopcommerce.com/boards/t/17711/unhandled-exception-request-is-not-available-in-this-context.aspx
            try
            {
                //we cannot load parser
                if (_browscapXmlHelper == null)
                    return false;

                var userAgent = _httpContext.Request.UserAgent;
                if (String.IsNullOrEmpty(userAgent))
                    return false;

                // check in already found cache
                bool result;
                if (_foundSearchEnginesCache.TryGetValue(userAgent, out result))
                    return true;

                // check cookie
                var cookie = _httpContext.Request.Cookies[IS_SEARCH_ENGINE_REQUEST_COOKIE_KEY];
                if (cookie != null)
                    return cookie.Value == "1";

                // check browscap
                result = _browscapXmlHelper.IsCrawler(userAgent);

                // if IS search engine, save to cache
                if (result)
                    _foundSearchEnginesCache.TryAdd(userAgent, true);

                // write cookie for subsequent requests
                _httpContext.Response.AppendCookie(new HttpCookie(IS_SEARCH_ENGINE_REQUEST_COOKIE_KEY)
                {
                    Value = result ? "1" : "0",
                    Expires = DateTime.Now.AddMonths(6),
                    HttpOnly = true
                });
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }
    }
}