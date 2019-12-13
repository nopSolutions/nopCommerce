using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097656165419186)]
    public class AddStockQuantityHistoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StockQuantityHistory)
                , nameof(StockQuantityHistory.ProductId)
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}