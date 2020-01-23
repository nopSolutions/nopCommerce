using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CustomerAttributeValueBuilder : BaseEntityBuilder<CustomerAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerAttributeValue.Name))
                    .AsString(400).NotNullable()
                .WithColumn(nameof(CustomerAttributeValue.CustomerAttributeId))
                    .AsInt32()
                    .ForeignKey<CustomerAttribute>();
        }

        #endregion
    }
}