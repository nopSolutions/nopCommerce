using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerAddressMappingBuilder : BaseEntityBuilder<CustomerAddressMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerAddressMapping.AddressId))
                    .AsInt32()
                    .ForeignKey<Address>()
                    .PrimaryKey()
                .WithColumn(nameof(CustomerAddressMapping.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                    .PrimaryKey();
        }

        #endregion
    }
}