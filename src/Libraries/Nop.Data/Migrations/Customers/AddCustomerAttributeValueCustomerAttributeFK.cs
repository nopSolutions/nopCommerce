using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097701504308129)]
    public class AddCustomerAttributeValueCustomerAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(CustomerAttributeValue))
                .ForeignColumn(nameof(CustomerAttributeValue.CustomerAttributeId))
                .ToTable(nameof(CustomerAttribute))
                .PrimaryColumn(nameof(CustomerAttribute.Id));

            Create.Index().OnTable(nameof(CustomerAttributeValue)).OnColumn(nameof(CustomerAttributeValue.CustomerAttributeId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}