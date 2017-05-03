using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly object _locker = new object();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        public UserAgentHelper(NopConfig config, IHttpContextAccessor httpContextAccessor)
        {
            this._config = config;
            this._httpContextAccessor = httpContextAccessor;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
        {
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            //no database created
            if (String.IsNullOrEmpty(_config.UserAgentStringsPath))
                return null;

            //prevent multi loading data
            lock (_locker)
            {
                //data can be loaded while we waited
                if (Singleton<BrowscapXmlHelper>.Instance != null)
                    return Singleton<BrowscapXmlHelper>.Instance;

                var userAgentStringsPath = CommonHelper.MapPath(_config.UserAgentStringsPath);
                var crawlerOnlyUserAgentStringsPath = string.IsNullOrEmpty(_config.CrawlerOnlyUserAgentStringsPath) ? string.Empty : CommonHelper.MapPath(_config.CrawlerOnlyUserAgentStringsPath);

                var browscapXmlHelper = new BrowscapXmlHelper(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
                Singleton<BrowscapXmlHelper>.Instance = browscapXmlHelper;

                return Singleton<BrowscapXmlHelper>.Instance;
            }
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine()
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                return false;

            //we put required logic in try-catch block
            //more info: http://www.nopcommerce.com/boards/t/17711/unhandled-exception-request-is-not-available-in-this-context.aspx
            try
            {
                var bowscapXmlHelper = GetBrowscapXmlHelper();

                //we cannot load parser
                if (bowscapXmlHelper == null)
                    return false;

                IHeaderDictionary headersDictionary = _httpContextAccessor.HttpContext.Request.Headers;
                var userAgent = headersDictionary[HeaderNames.UserAgent];
                return bowscapXmlHelper.IsCrawler(userAgent);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }
    }
}