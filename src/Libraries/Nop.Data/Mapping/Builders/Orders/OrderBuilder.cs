using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders;

/// <summary>
/// Represents a order entity builder
/// </summary>
public partial class OrderBuilder : NopEntityBuilder<Order>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Order.CustomOrderNumber)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Order.BillingAddressId)).AsInt32().ForeignKey<Address>(onDelete: Rule.None)
            .WithColumn(nameof(Order.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
            .WithColumn(nameof(Order.PickupAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None)
            .WithColumn(nameof(Order.ShippingAddressId)).AsInt32().Nullable().ForeignKey<Address>(onDelete: Rule.None)
            .WithColumn(nameof(Order.CustomerIp)).AsString(100).Nullable();
    }

    #endregion
}