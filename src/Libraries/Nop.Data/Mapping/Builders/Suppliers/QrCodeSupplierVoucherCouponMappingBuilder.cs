using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;

using Nop.Core.Domain.Suppliers;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class QrCodeSupplierVoucherCouponMappingBuilder : NopEntityBuilder<QrCodeSupplierVoucherCouponMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(QrCodeSupplierVoucherCouponMapping.SupplierVoucherCouponId)).AsInt32().ForeignKey<SupplierVoucherCoupon>()
                .WithColumn(nameof(QrCodeSupplierVoucherCouponMapping.StartDateTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(QrCodeSupplierVoucherCouponMapping.EndDateTime)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
