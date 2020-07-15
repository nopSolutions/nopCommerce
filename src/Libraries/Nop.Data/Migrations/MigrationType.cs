namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents an update migration type
    /// </summary>
    public enum UpdateMigrationType
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
        Settings = 15
    }
}