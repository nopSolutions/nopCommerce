using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerPasswordBuilder : BaseEntityBuilder<CustomerPassword>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerPassword.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}