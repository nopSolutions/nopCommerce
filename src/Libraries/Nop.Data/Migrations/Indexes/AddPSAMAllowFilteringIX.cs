using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123537559280391)]
    public class AddPSAMAllowFilteringIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_PSAM_AllowFiltering", NopMappingDefaults.ProductSpecificationAttributeTable,
                    i => i.Ascending(), nameof(ProductSpecificationAttribute.AllowFiltering))
                .Include(nameof(ProductSpecificationAttribute.ProductId))
                .Include(nameof(ProductSpecificationAttribute.SpecificationAttributeOptionId));
        }

        #endregion
    }
}