using System.Runtime.CompilerServices;
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
        #region Fields

        private readonly NopConfig _nopConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly object _locker = new object();

        #endregion

        #region Ctor
        
        public UserAgentHelper(NopConfig nopConfig, IHttpContextAccessor httpContextAccessor)
        {
            this._nopConfig = nopConfig;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Utilities

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
        {
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            //no database created
            if (string.IsNullOrEmpty(_nopConfig.UserAgentStringsPath))
                return null;

            //prevent multi loading data
            lock (_locker)
            {
                //data can be loaded while we waited
                if (Singleton<BrowscapXmlHelper>.Instance != null)
                    return Singleton<BrowscapXmlHelper>.Instance;

                var userAgentStringsPath = CommonHelper.MapPath(_nopConfig.UserAgentStringsPath);
                var crawlerOnlyUserAgentStringsPath = !string.IsNullOrEmpty(_nopConfig.CrawlerOnlyUserAgentStringsPath) ?
                    CommonHelper.MapPath(_nopConfig.CrawlerOnlyUserAgentStringsPath) : string.Empty;

                var browscapXmlHelper = new BrowscapXmlHelper(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
                Singleton<BrowscapXmlHelper>.Instance = browscapXmlHelper;

                return Singleton<BrowscapXmlHelper>.Instance;
            }
        }

        #endregion

        #region Methods

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

                var userAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];
                return bowscapXmlHelper.IsCrawler(userAgent);
            }
            catch { }

            return false;
        }

        #endregion
    }
}