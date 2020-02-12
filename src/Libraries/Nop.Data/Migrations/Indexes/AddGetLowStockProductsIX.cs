using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647941")]
    public class AddGetLowStockProductsIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_GetLowStockProducts", nameof(Product), i => i.Ascending(), nameof(Product.Deleted),
                nameof(Product.VendorId), nameof(Product.ProductTypeId), nameof(Product.ManageInventoryMethodId),
                nameof(Product.MinStockQuantity), nameof(Product.UseMultipleWarehouses));
        }

        #endregion
    }
}