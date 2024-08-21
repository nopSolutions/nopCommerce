using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

namespace Nop.Data.Migrations;

/// <summary>
/// Represents the migration manager
/// </summary>
public partial class MigrationManager : IMigrationManager
{
    #region Fields

    protected readonly IFilteringMigrationSource _filteringMigrationSource;
    protected readonly IMigrationRunner _migrationRunner;
    protected readonly IMigrationRunnerConventions _migrationRunnerConventions;
    protected readonly Lazy<IVersionLoader> _versionLoader;

    #endregion

    #region Ctor

    public MigrationManager(
        IFilteringMigrationSource filteringMigrationSource,
        IMigrationRunner migrationRunner,
        IMigrationRunnerConventions migrationRunnerConventions,
        Lazy<IVersionLoader> versionLoader)
    {
        _versionLoader = versionLoader;

        _filteringMigrationSource = filteringMigrationSource;
        _migrationRunner = migrationRunner;
        _migrationRunnerConventions = migrationRunnerConventions;
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
    protected virtual IEnumerable<IMigrationInfo> GetMigrations(Assembly assembly, bool exactlyProcessType,
        MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter, bool isApplied = false, bool isSchemaMigration = false)
    {
        if (exactlyProcessType && migrationProcessType == MigrationProcessType.NoMatter)
            throw new ArgumentException("Migration process type can't be NoMatter if exactlyProcessType set to true", nameof(migrationProcessType));

        //load all version data stored in the version table
        //we do this for avoid a problem with state of migration
        _versionLoader.Value.LoadVersionInfo();

        var migrations = _filteringMigrationSource
            .GetMigrations(t =>
            {
                var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();

                if (migrationAttribute is null)
                    return false;

                if (isSchemaMigration && !migrationAttribute.IsSchemaMigration)
                    return false;

                if (isApplied == !_versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                    return false;

                if (exactlyProcessType && migrationProcessType != migrationAttribute.TargetMigrationProcess)
                    return false;

                if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                    migrationProcessType != MigrationProcessType.NoMatter &&
                    migrationProcessType != migrationAttribute.TargetMigrationProcess)
                    return false;

                return assembly == null || t.Assembly == assembly;
            }) ?? Enumerable.Empty<IMigration>();

        return migrations.Select(_migrationRunnerConventions.GetMigrationInfoForMigration);
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
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, commitVersionOnly && migrationProcessType != MigrationProcessType.NoMatter, migrationProcessType).OrderBy(migration => migration.Version))
            ApplyUpMigration(migrationInfo, commitVersionOnly);
    }

    /// <summary>
    /// Executes an Up for schema unapplied migrations
    /// </summary>
    /// <param name="assembly">Assembly to find migrations</param>
    public virtual void ApplyUpSchemaMigrations(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, isSchemaMigration: true).OrderBy(migration => migration.Version))
            ApplyUpMigration(migrationInfo);
    }

    /// <summary>
    /// Executes a Down for all found (and applied) migrations
    /// </summary>
    /// <param name="assembly">Assembly to find the migration</param>
    public void ApplyDownMigrations(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var migrationInfo in GetMigrations(assembly, false, MigrationProcessType.NoMatter, true).OrderByDescending(migration => migration.Version))
            ApplyDownMigration(migrationInfo);
    }

    /// <summary>
    /// Executes down expressions for the passed migration
    /// </summary>
    /// <param name="migrationInfo">Migration to rollback</param>
    public void ApplyDownMigration(IMigrationInfo migrationInfo)
    {
        ArgumentNullException.ThrowIfNull(migrationInfo);

        _migrationRunner.Down(migrationInfo.Migration);
        _versionLoader.Value.DeleteVersion(migrationInfo.Version);
    }

    /// <summary>
    /// Executes up expressions for the passed migration
    /// </summary>
    /// <param name="migrationInfo">Migration to apply</param>
    /// <param name="commitVersionOnly">Commit only version information</param>
    public void ApplyUpMigration(IMigrationInfo migrationInfo, bool commitVersionOnly = false)
    {
        ArgumentNullException.ThrowIfNull(migrationInfo);

        if (!commitVersionOnly)
            _migrationRunner.Up(migrationInfo.Migration);
#if DEBUG
        if (!migrationInfo.IsNeedToApplyInDbOnDebugMode())
            return;
#endif
        _versionLoader.Value
            .UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
    }

    #endregion
}