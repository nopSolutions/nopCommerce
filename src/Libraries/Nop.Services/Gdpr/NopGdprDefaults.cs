using Nop.Core.Caching;

namespace Nop.Services.Gdpr
{
    /// <summary>
    /// Represents default values related to Gdpr services
    /// </summary>
    public static partial class NopGdprDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ConsentsAllCacheKey => new CacheKey("Nop.consents.all");

        #endregion
    }
}