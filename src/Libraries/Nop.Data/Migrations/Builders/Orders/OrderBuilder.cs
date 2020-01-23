using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class OrderBuilder : BaseEntityBuilder<Order>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Order.CustomOrderNumber)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Order.BillingAddressId))
                    .AsInt32()
                    .ForeignKey<Address>()
                .WithColumn(nameof(Order.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(Order.PickupAddressId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Address>()
                .WithColumn(nameof(Order.ShippingAddressId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Address>();

        }

        #endregion
    }
}