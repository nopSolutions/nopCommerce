using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.PriceLists;

/// <summary>
/// Represents a price list customer entity builder
/// </summary>
public partial class PriceListCustomerBuilder : NopEntityBuilder<PriceListCustomer>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PriceListCustomer.CustomerId)).AsInt32().ForeignKey<Customer>()
            .WithColumn(nameof(PriceListCustomer.PriceListId)).AsInt32().ForeignKey<PriceList>();
    }

    #endregion
}
