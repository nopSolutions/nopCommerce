using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerCustomerRoleMappingBuilder : BaseEntityBuilder<CustomerCustomerRoleMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerCustomerRoleMapping.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                    .PrimaryKey()
                .WithColumn(nameof(CustomerCustomerRoleMapping.CustomerRoleId))
                    .AsInt32()
                    .ForeignKey<CustomerRole>()
                    .PrimaryKey();
        }

        #endregion
    }
}