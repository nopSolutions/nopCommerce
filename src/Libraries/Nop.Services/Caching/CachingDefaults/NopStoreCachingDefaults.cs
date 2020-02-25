namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to stores services
    /// </summary>
    public static partial class NopStoreCachingDefaults
    {
        #region Store mappings

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string StoreMappingIdsByEntityIdNameCacheKey => "Nop.storemapping.ids.entityid-name-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string StoreMappingsByEntityIdNameCacheKey => "Nop.storemapping.entityid-name-{0}-{1}";

        #endregion

        #region Stores

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string StoresAllCacheKey => "Nop.stores.all";
        
        #endregion
    }
}