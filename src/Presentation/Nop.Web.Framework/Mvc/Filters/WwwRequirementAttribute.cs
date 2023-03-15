using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Data;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that checks WWW at the beginning of the URL and properly redirect if necessary
    /// </summary>
    public sealed class WwwRequirementAttribute : TypeFilterAttribute
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
        private class WwwRequirementFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            protected readonly IWebHelper _webHelper;
            protected readonly SeoSettings _seoSettings;

            #endregion

            #region Ctor

            public WwwRequirementFilter(IWebHelper webHelper,
                SeoSettings seoSettings)
            {
                _webHelper = webHelper;
                _seoSettings = seoSettings;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Check WWW prefix at the beginning of the URL and properly redirect if necessary
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <param name="withWww">Whether URL must start with WWW</param>
            private void RedirectRequest(AuthorizationFilterContext context, bool withWww)
            {
                //get scheme depending on securing connection
                var urlScheme = $"{_webHelper.GetCurrentRequestProtocol()}{Uri.SchemeDelimiter}";

                //compose start of URL with WWW
                var urlWith3W = $"{urlScheme}www.";

                //get requested URL
                var currentUrl = _webHelper.GetThisPageUrl(true);

                //whether requested URL starts with WWW
                var urlStartsWith3W = currentUrl.StartsWith(urlWith3W, StringComparison.OrdinalIgnoreCase);

                //page should have WWW prefix, so set 301 (permanent) redirection to URL with WWW
                if (withWww && !urlStartsWith3W)
                    context.Result = new RedirectResult(currentUrl.Replace(urlScheme, urlWith3W), true);

                //page shouldn't have WWW prefix, so set 301 (permanent) redirection to URL without WWW
                if (!withWww && urlStartsWith3W)
                    context.Result = new RedirectResult(currentUrl.Replace(urlWith3W, urlScheme), true);
            }

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            private void CheckWwwRequirement(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //only in GET requests, otherwise the browser might not propagate the verb and request body correctly.
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //ignore this rule for localhost
                if (_webHelper.IsLocalRequest(context.HttpContext.Request))
                    return;

                switch (_seoSettings.WwwRequirement)
                {
                    case WwwRequirement.WithWww:
                        //redirect to URL with starting WWW
                        RedirectRequest(context, true);
                        break;

                    case WwwRequirement.WithoutWww:
                        //redirect to URL without starting WWW
                        RedirectRequest(context, false);
                        break;

                    case WwwRequirement.NoMatter:
                        //do nothing
                        break;

                    default:
                        throw new NopException("Not supported WwwRequirement parameter");
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                CheckWwwRequirement(context);
                return Task.CompletedTask;
            }

            #endregion
        }

        #endregion
    }
}