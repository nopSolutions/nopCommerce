namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the type of migration process
    /// </summary>
    public enum MigrationProcessType
    {
        /// <summary>
        /// The type of migration process does not matter
        /// </summary>
        NoMatter,

        /// <summary>
        /// Installation
        /// </summary>
        Installation,

        /// <summary>
        /// Update
        /// </summary>
        Update
    }
}