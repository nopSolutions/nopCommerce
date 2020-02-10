using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 12:02:35:9280390")]
    public class AddPMMProductIdExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_PMM_ProductId_Extended", NopMappingDefaults.ProductManufacturerTable, i => i.Ascending(),
                    nameof(ProductManufacturer.ProductId), nameof(ProductManufacturer.IsFeaturedProduct))
                .Include(nameof(ProductManufacturer.ManufacturerId));
        }

        #endregion
    }
}