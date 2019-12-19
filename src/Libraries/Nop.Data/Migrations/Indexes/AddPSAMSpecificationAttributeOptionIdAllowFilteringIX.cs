using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123537559280392)]
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