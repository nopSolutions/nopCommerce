using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using LinqToDB.Tools;
using System.Data;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierVoucherCouponBuilder : NopEntityBuilder<SupplierVoucherCoupon>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SupplierVoucherCoupon.SystemName)).AsString(64).NotNullable()
                .WithColumn(nameof(SupplierVoucherCoupon.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(SupplierVoucherCoupon.Description)).AsString(512).Nullable()
                .WithColumn(nameof(SupplierVoucherCoupon.ImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(SupplierVoucherCoupon.SupplierId)).AsInt32().ForeignKey<Supplier>()
                .WithColumn(nameof(SupplierVoucherCoupon.SupplierShopId)).AsInt32().Nullable().ForeignKey<SupplierShop>().OnDelete(Rule.None)
                .WithColumn(nameof(SupplierVoucherCoupon.Percentage)).AsDecimal(9,2)
                .WithColumn(nameof(SupplierVoucherCoupon.MaxDiscountAmount)).AsDecimal(9, 2)
                .WithColumn(nameof(SupplierVoucherCoupon.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(SupplierVoucherCoupon.UpAmountCanUsed)).AsDecimal(9, 2)
                .WithColumn(nameof(SupplierVoucherCoupon.UpProfitCanUsed)).AsDecimal(9, 2)
                .WithColumn(nameof(SupplierVoucherCoupon.StartDateTimeUtc)).AsDateTime2().Nullable()




                ;
        }

        #endregion
    }
}
