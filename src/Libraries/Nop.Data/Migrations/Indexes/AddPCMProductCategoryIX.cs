using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037707")]
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