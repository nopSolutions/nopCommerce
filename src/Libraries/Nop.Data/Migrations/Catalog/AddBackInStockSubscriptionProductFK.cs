using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 11:47:54:8261630")]
    public class AddBackInStockSubscriptionProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BackInStockSubscription),
                nameof(BackInStockSubscription.ProductId),
                nameof(Product),
                nameof(Product.Id),
                Rule.Cascade);
        }

        #endregion
    }
}