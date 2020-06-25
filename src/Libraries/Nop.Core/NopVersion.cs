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
        public static string CurrentVersion => "4.40";

        public static string MinorVersion => "0";

        public static string FullVersion => CurrentVersion + "." + MinorVersion;
    }
}