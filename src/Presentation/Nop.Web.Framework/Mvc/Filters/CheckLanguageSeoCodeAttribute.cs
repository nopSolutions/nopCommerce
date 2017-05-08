using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
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
            private readonly IWorkContext _workContext;
            private readonly LocalizationSettings _localizationSettings;

            #endregion

            #region Ctor

            public CheckLanguageSeoCodeFilter(ILanguageService languageService,
                IWorkContext workContext,
                LocalizationSettings localizationSettings)
            {
                this._languageService = languageService;
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
                //TODO wrong implementation (doesn't work)
                if (context.RouteData == null || context.RouteData.Routers == null|| !context.RouteData.Routers.Any(route => route is LocalizedRoute))
                    return;

                //get request path
                var path = context.HttpContext.Request.Path.Value;

                //check whther it's already localized URL
                if (path.IsLocalizedUrl(context.HttpContext.Request.PathBase, true))
                {
                    //let's ensure that this language exists
                    var seoCode = path.GetLanguageSeoCodeFromUrl(context.HttpContext.Request.PathBase, true);
                    var urlLanguage = _languageService.GetAllLanguages()
                        .FirstOrDefault(language => seoCode.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                    if (urlLanguage != null && urlLanguage.Published)
                        return;

                    //doesn't exist, so redirect (not permanent) to the original page
                    path = path.RemoveLanguageSeoCodeFromRawUrl(context.HttpContext.Request.PathBase);
                    context.Result = new RedirectResult(string.Format("{0}{1}", path, context.HttpContext.Request.QueryString));
                    return;
                }

                //not localized yet, so redirect (permanent) to the page with working language SEO code
                path = path.AddLanguageSeoCodeToRawUrl(context.HttpContext.Request.PathBase, _workContext.WorkingLanguage);
                context.Result = new RedirectResult(string.Format("{0}{1}", path, context.HttpContext.Request.QueryString), true);
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