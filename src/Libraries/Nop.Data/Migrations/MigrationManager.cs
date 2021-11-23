using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class MigrationManager : IMigrationManager
    {
        #region Fields

        protected IFilteringMigrationSource FilteringMigrationSource { get; }
        protected IMigrationRunner MigrationRunner { get; }
        protected IMigrationRunnerConventions MigrationRunnerConventions { get; }
        protected Lazy<IVersionLoader> VersionLoader { get; }

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions,
            Lazy<IVersionLoader> versionLoader)
        {
            VersionLoader = versionLoader;

            FilteringMigrationSource = filteringMigrationSource;
            MigrationRunner = migrationRunner;
            MigrationRunnerConventions = migrationRunnerConventions;
        }

        #endregion

        #region Utils
        
        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process; pass null to load all migrations</param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        protected virtual IEnumerable<IMigrationInfo> GetMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
        {
            var migrations = FilteringMigrationSource
                .GetMigrations(t =>
                {
                    var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();
                    if (migrationAttribute is null || VersionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                        return false;

                    if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                        migrationProcessType != MigrationProcessType.NoMatter &&
                        migrationProcessType != migrationAttribute.TargetMigrationProcess)
                        return false;

                    return assembly == null || t.Assembly == assembly;
                }) ?? Enumerable.Empty<IMigration>();

            return migrations
                .Select(m => MigrationRunnerConventions.GetMigrationInfoForMigration(m))
                //.OrderBy(m => m.Migration.GetType().GetCustomAttribute<NopMigrationAttribute>().MigrationTarget)
                //.ThenBy(migration => migration.Version);
                .OrderBy(migration => migration.Version);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an Up for all found unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process</param>
        public virtual void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetMigrations(assembly, migrationProcessType))
            {
                MigrationRunner.Up(migrationInfo.Migration);

#if DEBUG
                if (!string.IsNullOrEmpty(migrationInfo.Description) &&
                    migrationInfo.Description.StartsWith(string.Format(NopMigrationDefaults.UpdateMigrationDescriptionPrefix, NopVersion.FULL_VERSION)))
                    continue;
#endif
                VersionLoader.Value
                    .UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
            }
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
                MigrationRunner.Down(migrationInfo.Migration);
                VersionLoader.Value.DeleteVersion(migrationInfo.Version);
            }
        }

        #endregion
    }
}