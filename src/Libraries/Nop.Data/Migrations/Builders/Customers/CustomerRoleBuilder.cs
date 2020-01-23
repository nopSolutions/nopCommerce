using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerRoleBuilder : BaseEntityBuilder<CustomerRole>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerRole.Name)).AsString(255).NotNullable()
                .WithColumn(nameof(CustomerRole.SystemName)).AsString(255).Nullable();
        }

        #endregion
    }
}