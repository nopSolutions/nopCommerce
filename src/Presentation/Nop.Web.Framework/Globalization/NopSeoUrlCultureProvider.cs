using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Globalization
{
    /// <summary>
    /// Determines the culture information for a request via the URL
    /// </summary>
    public class NopSeoUrlCultureProvider : RequestCultureProvider
    {
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();

            if(!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return await NullProviderCultureResult;

            //localized URLs are enabled, so try to get language from the requested page URL
            var (isLocalized, language) = await httpContext.Request.Path.Value.IsLocalizedUrlAsync(httpContext.Request.PathBase, false);

            if(!isLocalized)
                return await NullProviderCultureResult;

            return new ProviderCultureResult(language.LanguageCulture);
        }
    }
}
