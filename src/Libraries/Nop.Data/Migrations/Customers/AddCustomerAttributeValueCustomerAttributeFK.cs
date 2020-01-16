using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097701504308129)]
    public class AddCustomerAttributeValueCustomerAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(CustomerAttributeValue),
                nameof(CustomerAttributeValue.CustomerAttributeId),
                nameof(CustomerAttribute),
                nameof(CustomerAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}