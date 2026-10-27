using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.PriceLists;

/// <summary>
/// Represents a price list customer role entity builder
/// </summary>
public partial class PriceListCustomerRoleBuilder : NopEntityBuilder<PriceListCustomerRole>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PriceListCustomerRole.CustomerRoleId)).AsInt32().ForeignKey<CustomerRole>()
            .WithColumn(nameof(PriceListCustomerRole.PriceListId)).AsInt32().ForeignKey<PriceList>();
    }

    #endregion
}
