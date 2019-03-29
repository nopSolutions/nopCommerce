namespace Nop.Core.Redis
{
    /// <summary>
    /// Represents redis database number enumeration
    /// </summary>
    public enum RedisDatabaseNumber
    {
        /// <summary>
        /// Default database
        /// </summary>
        Default = -1,
        /// <summary>
        /// Database for caching
        /// </summary>
        Cache = 2,
        /// <summary>
        /// Database for plugins
        /// </summary>
        Plugin = 3
    }
}
