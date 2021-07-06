using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.IfDatabase;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class MigrationManager : IMigrationManager
    {
        #region Fields

        private readonly IFilteringMigrationSource _filteringMigrationSource;
        private readonly IMigrationRunner _migrationRunner;
        private readonly IMigrationRunnerConventions _migrationRunnerConventions;
        private readonly Lazy<IVersionLoader> _versionLoader;

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions)
        {
            _versionLoader = new Lazy<IVersionLoader>(() => EngineContext.Current.Resolve<IVersionLoader>());

            _filteringMigrationSource = filteringMigrationSource;
            _migrationRunner = migrationRunner;
            _migrationRunnerConventions = migrationRunnerConventions;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        private IEnumerable<IMigrationInfo> GetMigrations(Assembly assembly)
        {
            var migrations = _filteringMigrationSource.GetMigrations(t => assembly == null || t.Assembly == assembly) ?? Enumerable.Empty<IMigration>();

            return migrations.Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m)).OrderBy(migration => migration.Version);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        /// <param name="isUpdateProcess">Indicates whether the upgrade or installation process is ongoing. True - if an upgrade process</param>
        public void ApplyUpMigrations(Assembly assembly, bool isUpdateProcess = false)
        {
            if(assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            var migrations = GetMigrations(assembly);

            bool needToExecute(IMigrationInfo migrationInfo1)
            {
                var skip = migrationInfo1.Migration.GetType().GetCustomAttributes(typeof(SkipMigrationAttribute)).Any() || isUpdateProcess && migrationInfo1.Migration.GetType()
                    .GetCustomAttributes(typeof(SkipMigrationOnUpdateAttribute)).Any() || !isUpdateProcess && migrationInfo1.Migration.GetType()
                    .GetCustomAttributes(typeof(SkipMigrationOnInstallAttribute)).Any();

                return !skip;
            }

            foreach (var migrationInfo in migrations.Where(needToExecute))
                    _migrationRunner.MigrateUp(migrationInfo.Version);
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        public void ApplyDownMigrations(Assembly assembly)
        {
            if(assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            var migrations = GetMigrations(assembly).Reverse();

            foreach (var migrationInfo in migrations)
            {
                if (migrationInfo.Migration.GetType().GetCustomAttributes(typeof(SkipMigrationAttribute)).Any())
                    continue;

                _migrationRunner.Down(migrationInfo.Migration);
                _versionLoader.Value.DeleteVersion(migrationInfo.Version);
            }
        }

        #endregion
    }
}