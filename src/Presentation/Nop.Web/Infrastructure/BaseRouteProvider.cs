using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure
{
    public class BaseRouteProvider
    {
        protected string GetRouterPattern(IEndpointRouteBuilder endpointRouteBuilder, string seoCode = null)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    return $"{{{NopPathRouteDefaults.LanguageRouteValue}:maxlength(2):{NopPathRouteDefaults.LanguageParameterTransformer}=en}}/{seoCode}";
            }

            return seoCode ?? string.Empty;
        }
    }
}