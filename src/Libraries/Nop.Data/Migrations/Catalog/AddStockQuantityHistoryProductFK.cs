using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097656165419186)]
    public class AddStockQuantityHistoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(StockQuantityHistory))
                .ForeignColumn(nameof(StockQuantityHistory.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}