using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Extensions;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents filter attribute that checks SEO friendly URLs for multiple languages and properly redirect if necessary
    /// </summary>
    public class CheckLanguageSeoCodeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckLanguageSeoCodeAttribute() : base(typeof(CheckLanguageSeoCodeFilter))
        {
        }

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks SEO friendly URLs for multiple languages and properly redirect if necessary
        /// </summary>
        private class CheckLanguageSeoCodeFilter : IActionFilter
        {
            #region Fields

            private readonly ILanguageService _languageService;
            private readonly IWebHelper _webHelper;
            private readonly IWorkContext _workContext;
            private readonly LocalizationSettings _localizationSettings;

            #endregion

            #region Ctor

            public CheckLanguageSeoCodeFilter(ILanguageService languageService,
                IWebHelper webHelper,
                IWorkContext workContext,
                LocalizationSettings localizationSettings)
            {
                this._languageService = languageService;
                this._webHelper = webHelper;
                this._workContext = workContext;
                this._localizationSettings = localizationSettings;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null || context.HttpContext == null || context.HttpContext.Request == null)
                    return;

                if (!DataSettingsHelper.DatabaseIsInstalled())
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //whether SEO friendly URLs are enabled
                if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    return;

                //ensure that there is route and it is registered and localizable
                if (context.RouteData == null || context.RouteData.Routers == null || !context.RouteData.Routers.OfType<LocalizedRoute>().Any())
                    return;

                //get current page URL
                var pageUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
                var applicationPath = context.HttpContext.Request.PathBase.HasValue ?
                    context.HttpContext.Request.PathBase.Value : "/";

                //check whether it's already localized URL
                if (pageUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //let's ensure that this language exists and published
                    var seoCode = pageUrl.GetLanguageSeoCodeFromUrl(applicationPath, true);
                    if (!_languageService.GetAllLanguages()
                        .FirstOrDefault(language => seoCode.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase))
                        .Return(language => language.Published, false))
                    {
                        //if doesn't exist, redirect (not permanent) to the original page
                        pageUrl = pageUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                        context.Result = new RedirectResult(pageUrl);
                    }

                    return;
                }

                //not localized yet, so redirect to the page with working language SEO code
                pageUrl = pageUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
                context.Result = new RedirectResult(pageUrl, false);
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