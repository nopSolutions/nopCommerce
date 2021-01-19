using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;

namespace Nop.Web.Infrastructure
{
    public class BaseRouteProvider
    {
        protected string GetRouterPattern(IEndpointRouteBuilder endpointRouteBuilder, string seoCode = "")
        {
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    return $"{{language:maxlength(2):lang=en}}/{seoCode}";
            }

            return seoCode;
        }
    }
}