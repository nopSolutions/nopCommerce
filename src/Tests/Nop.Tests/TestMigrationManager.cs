using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using Nop.Data.Migrations;

namespace Nop.Tests
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class TestMigrationManager : IMigrationManager
    {
        #region Fields

        private readonly IMigrationRunner _migrationRunner;

        #endregion

        #region Ctor

        public TestMigrationManager(IMigrationRunner migrationRunner)
        {
            _migrationRunner = migrationRunner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an Up for all found unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process</param>
        /// <param name="commitVersionOnly">Commit only version information</param>
        public void ApplyUpMigrations(Assembly assembly,
            MigrationProcessType migrationProcessType = MigrationProcessType.Installation,
            bool commitVersionOnly = false)
        {
            _migrationRunner.MigrateUp(637200411689037680);
        }

        /// <summary>
        /// Executes a Down for all found (and applied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        public void ApplyDownMigrations(Assembly assembly)
        {
        }

        /// <summary>
        /// Executes down expressions for the passed migration
        /// </summary>
        /// <param name="migration">Migration to rollback</param>
        public void DownMigration(IMigration migration)
        {
        }

        /// <summary>
        /// Executes up expressions for the passed migration
        /// </summary>
        /// <param name="migration">Migration to apply</param>
        public void UpMigration(IMigration migration)
        {
        }

        #endregion
    }
}