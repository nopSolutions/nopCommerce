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
        if (!Schema.TableExist<FilterLevelValue>())
            Create.TableFor<FilterLevelValue>();

        if (!Schema.TableExist<FilterLevelValueProductMapping>())
            Create.TableFor<FilterLevelValueProductMapping>();
    }
}
