using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using LinqToDB.Tools;
using System.Data;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class ProductSupplierVoucherCouponMappingBuilder : NopEntityBuilder<ProductSupplierVoucherCouponMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.SupplierVoucherCouponId)).AsInt32().ForeignKey<SupplierVoucherCoupon>()
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>()
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.ProductAttributeValueId)).AsInt32().Nullable()
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ProductSupplierVoucherCouponMapping.EndDateTimeUtc)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
