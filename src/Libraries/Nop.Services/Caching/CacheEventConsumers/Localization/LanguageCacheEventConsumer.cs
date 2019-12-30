using Nop.Core.Domain.Localization;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Localization
{
    public partial class LanguageCacheEventConsumer : CacheEventConsumer<Language>
    {
        public override void ClearCashe(Language entity)
        {
            RemoveByPrefix(NopLocalizationCachingDefaults.LanguagesPrefixCacheKey);
        }
    }
}