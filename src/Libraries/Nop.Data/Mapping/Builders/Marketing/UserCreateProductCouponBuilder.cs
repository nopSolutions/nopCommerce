using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Core.Domain.Suppliers;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class UserCreateProductCouponBuilder : NopEntityBuilder<UserCreateProductCoupon>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserCreateProductCoupon.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(UserCreateProductCoupon.Description)).AsString(512).Nullable()
                .WithColumn(nameof(UserCreateProductCoupon.ProductId)).AsInt32().Nullable().ForeignKey<Product>().OnDelete(Rule.None)
                .WithColumn(nameof(UserCreateProductCoupon.ProductAttributeValueId)).AsInt32().Nullable()
                .WithColumn(nameof(UserCreateProductCoupon.SupplierId)).AsInt32().Nullable().ForeignKey<Supplier>().OnDelete(Rule.None)
                .WithColumn(nameof(UserCreateProductCoupon.SupplierShopId)).AsInt32().Nullable()
                .WithColumn(nameof(UserCreateProductCoupon.Url)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(UserCreateProductCoupon.Amount)).AsDecimal(9,2)
                .WithColumn(nameof(UserCreateProductCoupon.StartDateTimeUtc)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
