using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 12:02:35:9280392")]
    public class AddPSAMSpecificationAttributeOptionIdAllowFilteringIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_PSAM_SpecificationAttributeOptionId_AllowFiltering").OnTable(NameCompatibilityManager.GetTableName(typeof(ProductSpecificationAttribute)))
                .OnColumn(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId)).Ascending()
                .OnColumn(nameof(ProductSpecificationAttribute.AllowFiltering)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(ProductSpecificationAttribute.ProductId));
        }

        #endregion
    }
}