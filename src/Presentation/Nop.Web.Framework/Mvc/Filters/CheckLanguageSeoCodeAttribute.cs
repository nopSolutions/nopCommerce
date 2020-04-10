using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks SEO friendly URLs for multiple languages and properly redirect if necessary
    /// </summary>
    public sealed class CheckLanguageSeoCodeAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckLanguageSeoCodeAttribute() : base(typeof(CheckLanguageSeoCodeFilter))
        {
        }
        
        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks SEO friendly URLs for multiple languages and properly redirect if necessary
        /// </summary>
        private class CheckLanguageSeoCodeFilter : IActionFilter
        {
            #region Fields

            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;
            private readonly LocalizationSettings _localizationSettings;

            #endregion

            #region Ctor

            public CheckLanguageSeoCodeFilter(
                IWebHelper webHelper,
                IWorkContext workContext,
                LocalizationSettings localizationSettings)
            {
                _webHelper = webHelper;
                _workContext = workContext;
                _localizationSettings = localizationSettings;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //whether SEO friendly URLs are enabled
                if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    return;

                //ensure that this route is registered and localizable (LocalizedRoute in RouteProvider)
                if (context.RouteData.Values["language"] == null)
                    return;
                
                //check whether current page URL is already localized URL
                var pageUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
                if (pageUrl.IsLocalizedUrl(context.HttpContext.Request.PathBase, true, out var _))
                    return;

                //not localized yet, so redirect to the page with working language SEO code
                pageUrl = pageUrl.AddLanguageSeoCodeToUrl(context.HttpContext.Request.PathBase, true, _workContext.WorkingLanguage);
                context.Result = new LocalRedirectResult(pageUrl, false);
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}