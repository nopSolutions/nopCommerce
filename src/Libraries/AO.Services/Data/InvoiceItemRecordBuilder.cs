using AO.Services.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace AO.Services.Data
{
    public class InvoiceItemRecordBuilder : NopEntityBuilder<AOInvoiceItem>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AOInvoiceItem.InvoiceId)).AsInt32().ForeignKey("AOInvoice", "Id").NotNullable()
                .WithColumn(nameof(AOInvoiceItem.OrderItemId)).AsInt32().ForeignKey("OrderItem", "Id").NotNullable()
                .WithColumn(nameof(AOInvoiceItem.Credited)).AsBoolean()
                .WithColumn(nameof(AOInvoiceItem.Quantity)).AsInt32()
                .WithColumn(nameof(AOInvoiceItem.EAN)).AsString(15).Nullable();
        }
    }
}