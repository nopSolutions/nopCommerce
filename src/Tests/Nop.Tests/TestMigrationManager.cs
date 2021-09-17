using System.Reflection;
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
        public void ApplyUpMigrations(Assembly assembly,
            MigrationProcessType migrationProcessType = MigrationProcessType.Installation)
        {
            _migrationRunner.MigrateUp(637200411689037680);
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        public void ApplyDownMigrations(Assembly assembly)
        {
        }

        #endregion
    }
}