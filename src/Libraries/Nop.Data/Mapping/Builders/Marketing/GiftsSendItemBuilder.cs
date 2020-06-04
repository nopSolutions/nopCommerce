using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Core.Domain.Orders;
using System.Data;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class GiftsSendItemBuilder : NopEntityBuilder<GiftsSendItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GiftsSendItem.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(GiftsSendItem.Description)).AsString(255).Nullable()
                .WithColumn(nameof(GiftsSendItem.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(GiftsSendItem.ProductAttributeValueId)).AsInt32().Nullable().ForeignKey<ProductAttributeValue>().OnDelete(Rule.None)
                .WithColumn(nameof(GiftsSendItem.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>()
                .WithColumn(nameof(GiftsSendItem.ProfitScopeValue)).AsDecimal(9,2)
                .WithColumn(nameof(GiftsSendItem.TotalAmountScopeValue)).AsDecimal(9, 2)
                .WithColumn(nameof(GiftsSendItem.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(GiftsSendItem.Percentage)).AsDecimal(9, 2)
                .WithColumn(nameof(GiftsSendItem.MaxPercentageAmount)).AsDecimal(9, 2)
                .WithColumn(nameof(GiftsSendItem.GiftCardId)).AsInt32().Nullable().ForeignKey<GiftCard>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(GiftsSendItem.ExpiredDateTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(GiftsSendItem.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(GiftsSendItem.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
