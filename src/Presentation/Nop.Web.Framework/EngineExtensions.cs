using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework
{
    public static class EngineExtensions
    {
        public static ILocalizationService LocalizationService(this IEngine engine)
        {
            return engine.Resolve<ILocalizationService>();
        }
    }
}
