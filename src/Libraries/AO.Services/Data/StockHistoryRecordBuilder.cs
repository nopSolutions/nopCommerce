using AO.Services.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace AO.Services.Data
{
    public class StockHistoryRecordBuilder : NopEntityBuilder<AOStockHistory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AOStockHistory.CostPrice)).AsDecimal().NotNullable()
                .WithColumn(nameof(AOStockHistory.CreatedDate)).AsDateTime().NotNullable();
        }
    }
}
