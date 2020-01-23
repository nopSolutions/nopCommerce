using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerAttributeBuilder : BaseEntityBuilder<CustomerAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}