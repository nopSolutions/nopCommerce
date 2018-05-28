using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Seo;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that checks WWW at the beginning of the URL and properly redirect if necessary
    /// </summary>
    public class WwwRequirementAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public WwwRequirementAttribute() : base(typeof(WwwRequirementFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks WWW at the beginning of the URL and properly redirect if necessary
        /// </summary>
        private class WwwRequirementFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly IWebHelper _webHelper;
            private readonly SeoSettings _seoSettings;

            #endregion

            #region Ctor

            public WwwRequirementFilter(IWebHelper webHelper,
                SeoSettings seoSettings)
            {
                this._webHelper = webHelper;
                this._seoSettings = seoSettings;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Check WWW prefix at the beginning of the URL and properly redirect if necessary
            /// </summary>
            /// <param name="filterContext">Authorization filter context</param>
            /// <param name="withWww">Whether URL must start with WWW</param>
            protected void RedirectRequest(AuthorizationFilterContext filterContext, bool withWww)
            {
                //get scheme depending on securing connection
                var urlScheme = $"{_webHelper.CurrentRequestProtocol}{Uri.SchemeDelimiter}";

                //compose start of URL with WWW
                var urlWith3W = $"{urlScheme}www.";

                //get requested URL
                var currentUrl = _webHelper.GetThisPageUrl(true);

                //whether requested URL starts with WWW
                var urlStartsWith3W = currentUrl.StartsWith(urlWith3W, StringComparison.OrdinalIgnoreCase);

                //page should have WWW prefix, so set 301 (permanent) redirection to URL with WWW
                if (withWww && !urlStartsWith3W)
                    filterContext.Result = new RedirectResult(currentUrl.Replace(urlScheme, urlWith3W), true);

                //page shouldn't have WWW prefix, so set 301 (permanent) redirection to URL without WWW
                if (!withWww && urlStartsWith3W)
                    filterContext.Result = new RedirectResult(currentUrl.Replace(urlWith3W, urlScheme), true);
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

                //only in GET requests, otherwise the browser might not propagate the verb and request body correctly.
                if (!filterContext.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //ignore this rule for localhost
                if (_webHelper.IsLocalRequest(filterContext.HttpContext.Request))
                    return;

                switch (_seoSettings.WwwRequirement)
                {
                    case WwwRequirement.WithWww:
                        //redirect to URL with starting WWW
                        RedirectRequest(filterContext, true);
                        break;
                    case WwwRequirement.WithoutWww:
                        //redirect to URL without starting WWW
                        RedirectRequest(filterContext, false);
                        break;
                    case WwwRequirement.NoMatter:
                        //do nothing
                        break;
                    default:
                        throw new NopException("Not supported WwwRequirement parameter");
                }
            }

            #endregion
        }

        #endregion
    }
}