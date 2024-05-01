using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Shipping;

/// <summary>
/// Represents a shipping method state province mapping entity builder
/// </summary>
public partial class ShippingMethodStateProvinceMappingBuilder : NopEntityBuilder<ShippingMethodStateProvinceMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ShippingMethodStateProvinceMapping), nameof(ShippingMethodStateProvinceMapping.ShippingMethodId)))
            .AsInt32().PrimaryKey().ForeignKey<ShippingMethod>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ShippingMethodStateProvinceMapping), nameof(ShippingMethodStateProvinceMapping.CountryId)))
            .AsInt32().PrimaryKey().ForeignKey<Country>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ShippingMethodStateProvinceMapping), nameof(ShippingMethodStateProvinceMapping.StateProvinceId)))
            .AsInt32().PrimaryKey().ForeignKey<StateProvince>();
    }

    #endregion
}