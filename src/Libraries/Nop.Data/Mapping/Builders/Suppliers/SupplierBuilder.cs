using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierBuilder : NopEntityBuilder<Supplier>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Supplier.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(Supplier.Country)).AsString(15).Nullable()
                .WithColumn(nameof(Supplier.Province)).AsString(15).Nullable()
                .WithColumn(nameof(Supplier.City)).AsString(15).Nullable()
                .WithColumn(nameof(Supplier.District)).AsString(15).Nullable()
                .WithColumn(nameof(Supplier.Address)).AsString(128).Nullable()
                .WithColumn(nameof(Supplier.Contact)).AsString(15).Nullable()
                .WithColumn(nameof(Supplier.ContactNumber)).AsString(64).Nullable()
                .WithColumn(nameof(Supplier.Industry)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(Supplier.QrCodeUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(Supplier.BusinessLicenseUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(Supplier.BusinessLicenseCode)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(Supplier.TaxLicenseUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(Supplier.TaxLicenseCode)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(Supplier.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(Supplier.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
