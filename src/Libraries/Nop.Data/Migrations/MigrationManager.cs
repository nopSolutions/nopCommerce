using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class MigrationManager : IMigrationManager
    {
        #region Fields

        protected readonly IMigrationRunnerConventions _migrationRunnerConventions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        #region Ctor

        public MigrationManager(IMigrationRunnerConventions migrationRunnerConventions,
            IServiceScopeFactory serviceScopeFactory)
        {
            _migrationRunnerConventions = migrationRunnerConventions;
            _serviceScopeFactory = serviceScopeFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Up or Down process (depends on isApplied parameter)
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="exactlyProcessType">Indicate whether to found exactly specified migration process type; if it set to true, you should set the migrationProcessType parameter to different of NoMatter</param>
        /// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
        /// <param name="isApplied">A value indicating whether to select those migrations that have been applied</param>
        /// <param name="isSchemaMigration">Indicate whether to found only schema migration; if set to true will by returns only that migrations which has the IsSchemaMigration property set to true</param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        protected virtual IList<IMigrationInfo> GetMigrations(Assembly assembly, bool exactlyProcessType,
            MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter, bool isApplied = false, bool isSchemaMigration = false)
        {
            if (exactlyProcessType && migrationProcessType == MigrationProcessType.NoMatter)
                throw new ArgumentException("Migration process type can't be NoMatter if exactlyProcessType set to true", nameof(migrationProcessType));

            using var scope = _serviceScopeFactory.CreateScope();
            var versionLoader = scope.ServiceProvider.GetService<IVersionLoader>() ?? throw new NullReferenceException($"Can't get {nameof(IVersionLoader)} implementation from the scope");

            //load all version data stored in the version table
            //we do this for avoid a problem with state of migration
            versionLoader.LoadVersionInfo();

            var filteringMigrationSource = scope.ServiceProvider.GetService<IFilteringMigrationSource>() ?? throw new NullReferenceException($"Can't get {nameof(IFilteringMigrationSource)} implementation from the scope");

            var migrations = filteringMigrationSource
                .GetMigrations(t =>
                {
                    var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();

                    if (migrationAttribute is null)
                        return false;

                    if (isSchemaMigration && !migrationAttribute.IsSchemaMigration)
                        return false;

                    if (isApplied == !versionLoader.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                        return false;

                    if (exactlyProcessType && migrationProcessType != migrationAttribute.TargetMigrationProcess)
                        return false;

                    if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                        migrationProcessType != MigrationProcessType.NoMatter &&
                        migrationProcessType != migrationAttribute.TargetMigrationProcess)
                        return false;

                    return assembly == null || t.Assembly == assembly;
                }) ?? Enumerable.Empty<IMigration>();

            return migrations.Select(_migrationRunnerConventions.GetMigrationInfoForMigration).ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an Up for all found unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process</param>
        /// <param name="commitVersionOnly">Commit only version information</param>
        public virtual void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation, bool commitVersionOnly = false)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetMigrations(assembly, commitVersionOnly && migrationProcessType != MigrationProcessType.NoMatter, migrationProcessType).OrderBy(migration => migration.Version))
                ApplyUpMigration(migrationInfo, commitVersionOnly);
        }

        /// <summary>
        /// Executes an Up for schema unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        public virtual void ApplyUpSchemaMigrations(Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, isSchemaMigration: true).OrderBy(migration => migration.Version))
                ApplyUpMigration(migrationInfo);
        }

        /// <summary>
        /// Executes a Down for all found (and applied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        public void ApplyDownMigrations(Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, true).OrderByDescending(migration => migration.Version))
                ApplyDownMigration(migrationInfo);
        }

        /// <summary>
        /// Executes down expressions for the passed migration
        /// </summary>
        /// <param name="migrationInfo">Migration to rollback</param>
        public void ApplyDownMigration(IMigrationInfo migrationInfo)
        {
            if (migrationInfo is null)
                throw new ArgumentNullException(nameof(migrationInfo));

            using var scope = _serviceScopeFactory.CreateScope();

            var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>() ?? throw new NullReferenceException($"Can't get {nameof(IMigrationRunner)} implementation from the scope");

            var versionLoader = scope.ServiceProvider.GetService<IVersionLoader>() ?? throw new NullReferenceException($"Can't get {nameof(IVersionLoader)} implementation from the scope");

            migrationRunner.Down(migrationInfo.Migration);
            versionLoader.DeleteVersion(migrationInfo.Version);
        }

        /// <summary>
        /// Executes up expressions for the passed migration
        /// </summary>
        /// <param name="migrationInfo">Migration to apply</param>
        /// <param name="commitVersionOnly">Commit only version information</param>
        public void ApplyUpMigration(IMigrationInfo migrationInfo, bool commitVersionOnly = false)
        {
            if (migrationInfo is null)
                throw new ArgumentNullException(nameof(migrationInfo));

            using var scope = _serviceScopeFactory.CreateScope();

            if (!commitVersionOnly)
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>() ?? throw new NullReferenceException($"Can't get {nameof(IMigrationRunner)} implementation from the scope");

                migrationRunner.Up(migrationInfo.Migration);
            }
#if DEBUG
            if (!migrationInfo.IsNeedToApplyInDbOnDebugMode())
                return;
#endif
            var versionLoader = scope.ServiceProvider.GetService<IVersionLoader>() ?? throw new NullReferenceException($"Can't get {nameof(IVersionLoader)} implementation from the scope");

            versionLoader.UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
        }

        #endregion
    }
}