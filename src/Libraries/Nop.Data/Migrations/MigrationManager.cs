using System.Collections.Generic;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

/// <summary>
/// Represents the migration manager
/// </summary>
namespace Nop.Data.Migrations
{
    public class MigrationManager : IMigrationManager
    {
        #region Fields

        protected IFilteringMigrationSource _filteringMigrationSource;
        protected IMigrationRunner _migrationRunner;
        protected IMigrationRunnerConventions _migrationRunnerConventions;

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions)
        {
            _filteringMigrationSource = filteringMigrationSource;
            _migrationRunner = migrationRunner;
            _migrationRunnerConventions = migrationRunnerConventions;
        }

        #endregion

        #region Utils

        private IEnumerable<IMigration> GetMigrations(Assembly assembly, string[] tags)
        {
            return _filteringMigrationSource.GetMigrations(
                t => (assembly == null || t.Assembly == assembly) &&
                    (tags == null || _migrationRunnerConventions.HasRequestedTags(t, tags, true)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        public void ApplyUpMigrations(Assembly assembly = null, params string[] tags)
        {
            var migrations = GetMigrations(assembly, tags);

            foreach (var migration in migrations)
            {
                _migrationRunner.Up(migration);
            }
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration;
        /// leave null to search migration on the whole application pull</param>
        /// <param name="tags">Migration tags for filtering</param>
        public void ApplyDownMigrations(Assembly assembly = null, params string[] tags)
        {
            var migrations = GetMigrations(assembly, tags);

            foreach (var migration in migrations)
            {
                _migrationRunner.Down(migration);
            }
        }

        #endregion
    }
}