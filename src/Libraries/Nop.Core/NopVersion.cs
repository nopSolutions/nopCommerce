namespace Nop.Core
{
    /// <summary>
    /// Represents nopCommerce version
    /// </summary>
    public static class NopVersion
    {
        /// <summary>
        /// Gets the major store version
        /// </summary>
        public const string CURRENT_VERSION = "4.40";

        /// <summary>
        /// Gets the minor store version
        /// </summary>
        public const string MINOR_VERSION = "0";

        /// <summary>
        /// Gets the start date of developing the major version
        /// </summary>
        public const string VERSION_STARTED_ON = "2020-06-10";

        /// <summary>
        /// Gets the full store version
        /// </summary>
        public const string FULL_VERSION = CURRENT_VERSION + "." + MINOR_VERSION;
    }
}