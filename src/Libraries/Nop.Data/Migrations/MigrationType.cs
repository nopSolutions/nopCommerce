namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents a migration type
    /// </summary>
    public enum MigrationType
    {
        /// <summary>
        /// Database data
        /// </summary>
        Data = 5,

        /// <summary>
        /// Localization
        /// </summary>
        Localization = 10,

        /// <summary>
        /// Setting
        /// </summary>
        Setting = 15
    }
}