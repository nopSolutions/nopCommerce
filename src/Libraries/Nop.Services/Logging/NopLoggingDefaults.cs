using Nop.Core.Caching;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Represents default values related to logging services
    /// </summary>
    public static partial class NopLoggingDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ActivityTypeAllCacheKey => new CacheKey("Nop.activitytype.all");

        #endregion
    }
}