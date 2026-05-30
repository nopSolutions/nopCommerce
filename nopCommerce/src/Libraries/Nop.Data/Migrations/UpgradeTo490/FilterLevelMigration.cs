using FluentMigrator;
using Nop.Core.Domain.FilterLevels;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-07-03 00:00:00", "Filter level (YMM)")]
public class FilterLevelMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<FilterLevelValue>();
        this.CreateTableIfNotExists<FilterLevelValueProductMapping>();
    }
}
