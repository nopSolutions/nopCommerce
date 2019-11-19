using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097608748261630)]
    public class AddBackInStockSubscriptionProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BackInStockSubscription))
                .ForeignColumn(nameof(BackInStockSubscription.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));

        }

        #endregion
    }
}