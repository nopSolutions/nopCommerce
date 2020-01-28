using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:22:30:4308129")]
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