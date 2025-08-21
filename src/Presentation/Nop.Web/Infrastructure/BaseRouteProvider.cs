using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Represents base provider
/// </summary>
public partial class BaseRouteProvider
{
    /// <summary>
    /// Get pattern used to detect routes with language code
    /// </summary>
    /// <returns></returns>
    protected string GetLanguageRoutePattern()
    {
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                //by default we use empty for the language template, later it'll be specified with the working language code
                var code = string.Empty;
                return $"{{{NopRoutingDefaults.RouteValue.Language}:maxlength(2):{NopRoutingDefaults.LanguageParameterTransformer}={code}}}";
            }
        }

        return string.Empty;
    }
}