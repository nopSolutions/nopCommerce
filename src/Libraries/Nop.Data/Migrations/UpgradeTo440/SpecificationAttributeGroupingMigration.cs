using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo440;

[NopSchemaMigration("2020/03/08 11:26:08:9037680", "Specification attribute grouping")]
public class SpecificationAttributeGroupingMigration : ForwardOnlyMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.TableExist<SpecificationAttributeGroup>())
            Create.TableFor<SpecificationAttributeGroup>();

        if (!Schema.ColumnExist<SpecificationAttribute>(t => t.SpecificationAttributeGroupId))
        {
            //add new column
            Alter.AddColumnFor<SpecificationAttribute>(t => t.SpecificationAttributeGroupId)
                .AsInt32()
                .Nullable()
                .ForeignKey<SpecificationAttributeGroup>();
        }
    }

    #endregion
}