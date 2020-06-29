using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierShopBuilder : NopEntityBuilder<SupplierShop>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SupplierShop.SupplierId)).AsInt32().ForeignKey<Supplier>()
                .WithColumn(nameof(SupplierShop.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(SupplierShop.Description)).AsString(255).Nullable()
                .WithColumn(nameof(SupplierShop.Content)).AsString(1024).Nullable()
                .WithColumn(nameof(SupplierShop.Country)).AsString(15).Nullable()
                .WithColumn(nameof(SupplierShop.ImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(SupplierShop.ThumbImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(SupplierShop.Province)).AsString(15).Nullable()
                .WithColumn(nameof(SupplierShop.City)).AsString(15).Nullable()
                .WithColumn(nameof(SupplierShop.District)).AsString(15).Nullable()
                .WithColumn(nameof(SupplierShop.Address)).AsString(128).Nullable()
                .WithColumn(nameof(SupplierShop.Contact)).AsString(15).Nullable()
                .WithColumn(nameof(SupplierShop.ContactNumber)).AsString(64).Nullable()
                .WithColumn(nameof(SupplierShop.QrCodeUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(SupplierShop.Latitude)).AsDecimal(9, 6)
                .WithColumn(nameof(SupplierShop.Longitude)).AsDecimal(9, 6)
                .WithColumn(nameof(SupplierShop.Precision)).AsDecimal(9, 6)
                .WithColumn(nameof(SupplierShop.OpenTime)).AsString(128).Nullable()
                .WithColumn(nameof(SupplierShop.Notices)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
