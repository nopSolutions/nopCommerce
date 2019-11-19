using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097656165419187)]
    public class AddStockQuantityHistoryWarehouseFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(StockQuantityHistory))
                .ForeignColumn(nameof(StockQuantityHistory.WarehouseId))
                .ToTable(nameof(Warehouse))
                .PrimaryColumn(nameof(Warehouse.Id));
        }

        #endregion
    }
}