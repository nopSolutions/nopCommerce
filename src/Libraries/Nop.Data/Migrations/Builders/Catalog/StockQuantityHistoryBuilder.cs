using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class StockQuantityHistoryBuilder : BaseEntityBuilder<StockQuantityHistory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StockQuantityHistory.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>()
                .WithColumn(nameof(StockQuantityHistory.WarehouseId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Warehouse>();
        }

        #endregion
    }
}