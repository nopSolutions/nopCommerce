using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;

namespace Nop.Tests;

/// <summary>
/// Represents object for the applying additional migrations for a test project
/// </summary>
[NopMigration("2000-12-31 23:59:59", "Test schema installation", MigrationProcessType.Installation)]
public abstract class TestMigration : ForwardOnlyMigration
{
    public static void ApplyMigrations(Assembly assembly, IMigrationManager migrationManager) =>
        migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Installation);

    public static void ApplyMigrations(ITypeFinder typeFinder, IMigrationManager migrationManager)
    {
        foreach (var assembly in typeFinder.FindClassesOfType<TestMigration>().Select(x => x.Assembly).Distinct())
            ApplyMigrations(assembly, migrationManager);
    }
}

public abstract class TestMigration<T> : TestMigration where T : MigrationBase, new()
{
    protected T _target = new();

    public override void GetUpExpressions(IMigrationContext context)
    {
        _target.GetUpExpressions(context);
    }

    public override void Up()
    {
        _target.Up();
    }
}
