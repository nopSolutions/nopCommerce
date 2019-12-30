using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647942)]
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