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
        /// 0 - nopCommerce version
        /// 1 - update migration type
        /// </remarks>
        /// </summary>
        public static string UpdateMigrationDescription => "nopCommerce version {0}. Update {1}";
    }
}
