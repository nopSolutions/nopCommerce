using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 01:06:56:5419186")]
    public class AddStockQuantityHistoryProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StockQuantityHistory), 
                nameof(StockQuantityHistory.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}