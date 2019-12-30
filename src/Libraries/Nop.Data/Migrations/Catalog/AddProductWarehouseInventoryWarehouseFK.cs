using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097650980051781)]
    public class AddProductWarehouseInventoryWarehouseFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductWarehouseInventory),
                nameof(ProductWarehouseInventory.WarehouseId), 
                nameof(Warehouse), 
                nameof(Warehouse.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}