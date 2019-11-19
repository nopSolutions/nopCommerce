using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097650980051781)]
    public class AddProductWarehouseInventoryWarehouseFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductWarehouseInventory))
                .ForeignColumn(nameof(ProductWarehouseInventory.WarehouseId))
                .ToTable(nameof(Warehouse))
                .PrimaryColumn(nameof(Warehouse.Id));
        }

        #endregion
    }
}