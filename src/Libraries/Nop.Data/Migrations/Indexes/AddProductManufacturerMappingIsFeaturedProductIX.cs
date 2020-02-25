using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647938")]
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