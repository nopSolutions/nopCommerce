using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037707)]
    public class AddPCMProductCategoryIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PCM_Product_and_Category", NopMappingDefaults.ProductCategoryTable, i => i.Ascending(),
                nameof(ProductCategory.CategoryId), nameof(ProductCategory.ProductId));
        }

        #endregion
    }
}