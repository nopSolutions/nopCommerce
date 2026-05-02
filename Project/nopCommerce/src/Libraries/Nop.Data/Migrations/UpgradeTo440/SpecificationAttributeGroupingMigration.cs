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
        this.CreateTableIfNotExists<SpecificationAttributeGroup>();

        //add new column
        this.AddOrAlterForeignKeyColumnFor<SpecificationAttribute, SpecificationAttributeGroup>(t =>
            t.SpecificationAttributeGroupId).Nullable();
    }

    #endregion
}