using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Globalization
{
    /// <summary>
    /// Determines the culture information for a request via the value of the Accept-Language header.
    /// </summary>
    public class NopHeaderRequestCultureProvider : AcceptLanguageHeaderRequestCultureProvider
    {
        /// <returns>A task that represents the asynchronous operation</returns>
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

            if(!localizationSettings.AutomaticallyDetectLanguage || 
                httpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName] != null)
            {
                return NullProviderCultureResult;
            }

            return base.DetermineProviderCultureResult(httpContext);
        }
    }
}