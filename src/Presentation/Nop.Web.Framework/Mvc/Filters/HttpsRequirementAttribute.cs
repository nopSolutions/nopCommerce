using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that checks whether current connection is secured and properly redirect if necessary
    /// </summary>
    public sealed class HttpsRequirementAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public HttpsRequirementAttribute() : base(typeof(HttpsRequirementFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
        /// </summary>
        private class HttpsRequirementFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly IStoreContext _storeContext;
            private readonly IWebHelper _webHelper;

            #endregion

            #region Ctor

            public HttpsRequirementFilter(IStoreContext storeContext, IWebHelper webHelper)
            {
                _storeContext = storeContext;
                _webHelper = webHelper;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Check whether current connection is secured and properly redirect if necessary
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            /// <param name="useSsl">Whether the page should be secured</param>
            protected void RedirectRequest(AuthorizationFilterContext filterContext, bool useSsl)
            {
                //whether current connection is secured
                var currentConnectionSecured = _webHelper.IsCurrentConnectionSecured();

                //page should be secured, so redirect (permanent) to HTTPS version of page
                if (useSsl && !currentConnectionSecured)
                    filterContext.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, true), true);

                //page shouldn't be secured, so redirect (permanent) to HTTP version of page
                if (!useSsl && currentConnectionSecured)
                    filterContext.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, false), true);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (filterContext.HttpContext.Request == null)
                    return;

                //only in GET requests, otherwise the browser might not propagate the verb and request body correctly
                if (!filterContext.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                RedirectRequest(filterContext, _storeContext.CurrentStore.SslEnabled);
            }

            #endregion
        }

        #endregion
    }
}