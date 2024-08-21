using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Builders.Stores;

/// <summary>
/// Represents a store entity builder
/// </summary>
public partial class StoreBuilder : NopEntityBuilder<Store>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Store.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Store.Url)).AsString(400).NotNullable()
            .WithColumn(nameof(Store.Hosts)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyName)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyAddress)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyPhoneNumber)).AsString(1000).Nullable()
            .WithColumn(nameof(Store.CompanyVat)).AsString(1000).Nullable();
    }

    #endregion
}