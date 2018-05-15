namespace Nop.Core.Caching
{
    /// <summary>
    /// Redis settings
    /// </summary>
    public static class RedisConfiguration
    {
        /// <summary>
        /// Get the key used to store the protection key list (used with the PersistDataProtectionKeysToRedis option enabled)
        /// </summary>
        public static string DataProtectionKeysName => "Nop.DataProtectionKeys";
    }
}