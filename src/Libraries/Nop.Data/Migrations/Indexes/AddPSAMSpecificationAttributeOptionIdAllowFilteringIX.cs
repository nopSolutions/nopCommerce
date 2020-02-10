using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 12:02:35:9280392")]
    public class AddPSAMSpecificationAttributeOptionIdAllowFilteringIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PSAM_SpecificationAttributeOptionId_AllowFiltering",
                    NopMappingDefaults.ProductSpecificationAttributeTable, i => i.Ascending(),
                    nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId),
                    nameof(ProductSpecificationAttribute.AllowFiltering))
                .Include(nameof(ProductSpecificationAttribute.ProductId));
        }

        #endregion
    }
}