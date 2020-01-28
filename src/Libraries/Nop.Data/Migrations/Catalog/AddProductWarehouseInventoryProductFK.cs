using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:58:18:0051780")]
    public class AddProductWarehouseInventoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductWarehouseInventory), 
                nameof(ProductWarehouseInventory.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}