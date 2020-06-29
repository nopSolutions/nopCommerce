using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class UserSupplierVoucherCouponBuilder : NopEntityBuilder<UserSupplierVoucherCoupon>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserSupplierVoucherCoupon.WUserId)).AsInt32().PrimaryKey().ForeignKey<WUser>()
                .WithColumn(nameof(UserSupplierVoucherCoupon.SupplierVoucherCouponId)).AsInt32().PrimaryKey().ForeignKey<SupplierVoucherCoupon>()
                ;
        }

        #endregion
    }
}
