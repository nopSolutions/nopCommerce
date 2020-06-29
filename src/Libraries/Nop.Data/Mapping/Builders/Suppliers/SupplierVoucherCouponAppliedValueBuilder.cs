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
    public partial class SupplierVoucherCouponAppliedValueBuilder : NopEntityBuilder<SupplierVoucherCouponAppliedValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SupplierVoucherCouponAppliedValue.SupplierVoucherCouponId)).AsInt32().ForeignKey<SupplierVoucherCoupon>()
                ;
        }

        #endregion
    }
}
