using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;

namespace Nop.Services.Configuration
{
    /// <summary>
    /// Represents default values related to settings
    /// </summary>
    public static partial class NopSettingsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new("Nop.setting.all.dictionary.", NopEntityCacheDefaults<Setting>.Prefix);

        #endregion
    }
}