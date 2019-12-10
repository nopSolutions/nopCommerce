using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097650980051780)]
    public class AddProductWarehouseInventoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductWarehouseInventory))
                .ForeignColumn(nameof(ProductWarehouseInventory.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

            Create.Index().OnTable(nameof(ProductWarehouseInventory)).OnColumn(nameof(ProductWarehouseInventory.ProductId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}