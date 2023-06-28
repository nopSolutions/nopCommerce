namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents cache configuration parameters
    /// </summary>
    public partial class CacheConfig : IConfig
    {
        /// <summary>
        /// Gets or sets the default cache time in minutes
        /// </summary>
        public int DefaultCacheTime { get; protected set; } = 60;

        /// <summary>
        /// Gets or sets the short term cache time in minutes
        /// </summary>
        public int ShortTermCacheTime { get; protected set; } = 3;

        /// <summary>
        /// Gets or sets the bundled files cache time in minutes
        /// </summary>
        public int BundledFilesCacheTime { get; protected set; } = 120;
    }
}