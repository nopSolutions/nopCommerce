using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647942")]
    public class AddPMMProductManufacturerIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_PMM_Product_and_Manufacturer", NopMappingDefaults.ProductManufacturerTable,
                i => i.Ascending(), nameof(ProductManufacturer.ManufacturerId), nameof(ProductManufacturer.ProductId));
        }

        #endregion
    }
}