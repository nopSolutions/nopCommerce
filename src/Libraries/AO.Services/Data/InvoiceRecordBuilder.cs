using AO.Services.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace AO.Services.Data
{
    public class InvoiceRecordBuilder : NopEntityBuilder<AOInvoice>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(AOInvoice.OrderId)).AsInt32()
                .WithColumn(nameof(AOInvoice.CustomerId)).AsInt32()
                .WithColumn(nameof(AOInvoice.InvoiceDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(AOInvoice.PaymentDate)).AsDateTime().NotNullable()
                .WithColumn(nameof(AOInvoice.InvoiceTax)).AsDecimal()
                .WithColumn(nameof(AOInvoice.InvoiceDiscount)).AsDecimal()
                .WithColumn(nameof(AOInvoice.InvoiceTotal)).AsDecimal()
                .WithColumn(nameof(AOInvoice.InvoiceRefundedAmount)).AsDecimal()
                .WithColumn(nameof(AOInvoice.InvoiceShipping)).AsDecimal()
                .WithColumn(nameof(AOInvoice.CurrencyRate)).AsDecimal()
                .WithColumn(nameof(AOInvoice.TrackingNumber)).AsString(100).Nullable()
                .WithColumn(nameof(AOInvoice.CustomerCurrencyCode)).AsString(10).Nullable()
                .WithColumn(nameof(AOInvoice.InvoiceIsPaid)).AsBoolean()
                .WithColumn(nameof(AOInvoice.InvoiceIsManual)).AsBoolean();
        }
    }
}