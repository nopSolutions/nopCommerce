using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Suppliers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class UserAssetIncomeHistoryBuilder : NopEntityBuilder<UserAssetIncomeHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserAssetIncomeHistory.Name)).AsString(64).Nullable()
                .WithColumn(nameof(UserAssetIncomeHistory.Description)).AsString(512).Nullable()
                .WithColumn(nameof(UserAssetIncomeHistory.Remark)).AsString(512).Nullable()
                .WithColumn(nameof(UserAssetIncomeHistory.GiftsSendItemId)).AsInt32().Nullable()
                .WithColumn(nameof(UserAssetIncomeHistory.WUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(UserAssetIncomeHistory.GiverId)).AsInt32().Nullable().ForeignKey<WUser>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetIncomeHistory.PurchasedWithOrderItemId)).AsInt32().Nullable().ForeignKey<OrderItem>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetIncomeHistory.SupplierId)).AsInt32().Nullable().ForeignKey<Supplier>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetIncomeHistory.SupplierShopId)).AsInt32().Nullable().ForeignKey<SupplierShop>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetIncomeHistory.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAssetIncomeHistory.UsedValue)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAssetIncomeHistory.GiftCardCouponCode)).AsString(64).Nullable()
                .WithColumn(nameof(UserAssetIncomeHistory.StartDateTimeUtc)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
