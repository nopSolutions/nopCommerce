using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037694")]
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