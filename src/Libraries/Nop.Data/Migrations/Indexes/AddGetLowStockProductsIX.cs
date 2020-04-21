using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647941")]
    public class AddGetLowStockProductsIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_GetLowStockProducts").OnTable(nameof(Product))
                .OnColumn(nameof(Product.Deleted)).Ascending()
                .OnColumn(nameof(Product.VendorId)).Ascending()
                .OnColumn(nameof(Product.ProductTypeId)).Ascending()
                .OnColumn(nameof(Product.ManageInventoryMethodId)).Ascending()
                .OnColumn(nameof(Product.MinStockQuantity)).Ascending()
                .OnColumn(nameof(Product.UseMultipleWarehouses)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}