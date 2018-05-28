using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Security;
using Nop.Web.Framework.Security;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that checks whether current connection is secured and properly redirect if necessary
    /// </summary>
    public class HttpsRequirementAttribute : TypeFilterAttribute
    {
        #region Fields

        private readonly SslRequirement _sslRequirement;

        #endregion

        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="sslRequirement">Whether the page should be secured</param>
        public HttpsRequirementAttribute(SslRequirement sslRequirement) : base(typeof(HttpsRequirementFilter))
        {
            this._sslRequirement = sslRequirement;
            this.Arguments = new object[] { sslRequirement };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the page should be secured
        /// </summary>
        public SslRequirement SslRequirement => _sslRequirement;

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter confirming that checks whether current connection is secured and properly redirect if necessary
        /// </summary>
        private class HttpsRequirementFilter : IAuthorizationFilter
        {
            #region Fields

            private SslRequirement _sslRequirement;
            private readonly IStoreContext _storeContext;
            private readonly IWebHelper _webHelper;
            private readonly SecuritySettings _securitySettings;

            #endregion

            #region Ctor

            public HttpsRequirementFilter(SslRequirement sslRequirement,
                IStoreContext storeContext,
                IWebHelper webHelper,
                SecuritySettings securitySettings)
            {
                this._sslRequirement = sslRequirement;
                this._storeContext = storeContext;
                this._webHelper = webHelper;
                this._securitySettings = securitySettings;
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
                if (useSsl && !currentConnectionSecured && _storeContext.CurrentStore.SslEnabled)
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

                //check whether this filter has been overridden for the Action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<HttpsRequirementAttribute>().FirstOrDefault();

                var sslRequirement = actionFilter?.SslRequirement ?? _sslRequirement;

                //whether all pages will be forced to use SSL no matter of the passed value
                if (_securitySettings.ForceSslForAllPages)
                    sslRequirement = SslRequirement.Yes;

                switch (sslRequirement)
                {
                    case SslRequirement.Yes:
                        //redirect to HTTPS page
                        RedirectRequest(filterContext, true);
                        break;
                    case SslRequirement.No:
                        //redirect to HTTP page
                        RedirectRequest(filterContext, false);
                        break;
                    case SslRequirement.NoMatter:
                        //do nothing
                        break;
                    default:
                        throw new NopException("Not supported SslRequirement parameter");
                }
            }

            #endregion
        }

        #endregion
    }
}