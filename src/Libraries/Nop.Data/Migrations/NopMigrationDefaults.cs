namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents default values related to migration process
    /// </summary>
    public static partial class NopMigrationDefaults
    {
        /// <summary>
        /// Gets the supported datetime formats
        /// </summary>
        public static string[] DateFormats { get; } = { "yyyy-MM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss:fffffff", "yyyy.MM.dd HH:mm:ss:fffffff", "yyyy/MM/dd HH:mm:ss:fffffff" };

        /// <summary>
        /// Gets the format string to create the description of update migration
        /// <remarks>
        /// 0 - nopCommerce full version
        /// 1 - update migration type
        /// </remarks>
        /// </summary>
        public static string UpdateMigrationDescription { get; } = "nopCommerce version {0}. Update {1}";

        /// <summary>
        /// Gets the format string to create the description prefix of update migrations
        /// <remarks>
        /// 0 - nopCommerce full version
        /// </remarks>
        /// </summary>
        public static string UpdateMigrationDescriptionPrefix { get; } = "nopCommerce version {0}. Update";
    }
}
