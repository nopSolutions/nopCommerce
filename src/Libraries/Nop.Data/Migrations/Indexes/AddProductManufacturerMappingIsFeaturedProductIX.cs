using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647938)]
    public class AddProductManufacturerMappingIsFeaturedProductIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_Manufacturer_Mapping_IsFeaturedProduct",
                NopMappingDefaults.ProductManufacturerTable, i => i.Ascending(),
                nameof(ProductManufacturer.IsFeaturedProduct));
        }

        #endregion
    }
}