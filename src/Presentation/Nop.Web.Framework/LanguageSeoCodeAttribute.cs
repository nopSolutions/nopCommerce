#if NET451
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Attribute which ensures that store URL contains a language SEO code if "SEO friendly URLs with multiple languages" setting is enabled
    /// </summary>
    public class LanguageSeoCodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return;
            
            //ensure that this route is registered and localizable (LocalizedRoute in RouteProvider.cs)
            if (filterContext.RouteData == null || filterContext.RouteData.Route == null || !(filterContext.RouteData.Route is LocalizedRoute))
                return;


            //process current URL
            var pageUrl = request.RawUrl;
            string applicationPath = request.ApplicationPath;
            if (pageUrl.IsLocalizedUrl(applicationPath, true))
            {
                //already localized URL
                //let's ensure that this language exists
                var seoCode = pageUrl.GetLanguageSeoCodeFromUrl(applicationPath, true);
                
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var language = languageService.GetAllLanguages()
                    .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (language != null && language.Published)
                {
                    //exists
                    return;
                }
                else
                {
                    //doesn't exist. redirect to the original page (not permanent)
                    pageUrl = pageUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                    filterContext.Result = new RedirectResult(pageUrl);
                }
            }
            //add language code to URL
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            pageUrl = pageUrl.AddLanguageSeoCodeToRawUrl(applicationPath, workContext.WorkingLanguage);
            //301 (permanent) redirection
            filterContext.Result = new RedirectResult(pageUrl, true);
        }
    }
}
#endif