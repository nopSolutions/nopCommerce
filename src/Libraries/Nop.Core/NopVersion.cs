namespace Nop.Core
{
    /// <summary>
    /// Represents nopCommerce version
    /// </summary>
    public static class NopVersion
    {
        /// <summary>
        /// Gets or sets the store version
        /// </summary>
        public static string CurrentVersion { get; } = "4.40";

        public static string MinorVersion { get; } = "0";

        public static string FullVersion { get; } = $"{CurrentVersion}.{MinorVersion}";
    }
}