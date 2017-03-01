using System;
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
                return _browscapXmlHelper.IsCrawler(userAgent);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return false;
        }
    }
}