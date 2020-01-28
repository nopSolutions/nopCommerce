using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:14:20:9436789")]
    public class AddOrderItemProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(OrderItem),
                nameof(OrderItem.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}