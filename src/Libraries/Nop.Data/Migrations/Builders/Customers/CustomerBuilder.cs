using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerBuilder : BaseEntityBuilder<Customer>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table

            .WithColumn(nameof(Customer.Username)).AsString(1000).Nullable()
            .WithColumn(nameof(Customer.Email)).AsString(1000).Nullable()
            .WithColumn(nameof(Customer.EmailToRevalidate)).AsString(1000).Nullable()
            .WithColumn(nameof(Customer.SystemName)).AsString(400).Nullable()
            .WithColumn(nameof(Customer.BillingAddressId))
                .AsInt32()
                .ForeignKey<Address>()
                .Nullable()
            .WithColumn(nameof(Customer.ShippingAddressId))
                .AsInt32()
                .ForeignKey<Address>()
                .Nullable();
        }

        #endregion
    }
}