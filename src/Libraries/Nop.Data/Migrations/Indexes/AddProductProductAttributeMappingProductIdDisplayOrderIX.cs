using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037694)]
    public class AddProductProductAttributeMappingProductIdDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder",
                NopMappingDefaults.ProductProductAttributeTable, i => i.Ascending(),
                nameof(ProductAttributeMapping.ProductId), nameof(ProductAttributeMapping.DisplayOrder));
        }

        #endregion
    }
}