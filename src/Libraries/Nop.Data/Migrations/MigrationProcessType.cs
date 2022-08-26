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
        Update,

        /// <summary>
        /// Apply migration right after the migration runner will become available
        /// </summary>
        ///<remarks>
        /// Avoid using dependency injection in migrations that are marked by this type of migration process,
        /// because many dependencies are not registered yet.
        ///</remarks>
        NoDependencies
    }
}