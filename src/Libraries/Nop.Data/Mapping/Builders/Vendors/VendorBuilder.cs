using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Builders.Vendors;

/// <summary>
/// Represents a vendor entity builder
/// </summary>
public partial class VendorBuilder : NopEntityBuilder<Vendor>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Vendor.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Vendor.Email)).AsString(400).Nullable()
            .WithColumn(nameof(Vendor.MetaKeywords)).AsString(400).Nullable()
            .WithColumn(nameof(Vendor.MetaTitle)).AsString(400).Nullable()
            .WithColumn(nameof(Vendor.PageSizeOptions)).AsString(200).Nullable();
    }

    #endregion
}