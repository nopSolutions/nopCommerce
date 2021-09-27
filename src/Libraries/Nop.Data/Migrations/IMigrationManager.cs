using System.Reflection;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents a migration manager
    /// </summary>
    public interface IMigrationManager
    {
        /// <summary>
        /// Executes an Up for all found unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process</param>
        void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation);

        /// <summary>
        /// Executes an Down migration
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        void ApplyDownMigrations(Assembly assembly);
    }
}