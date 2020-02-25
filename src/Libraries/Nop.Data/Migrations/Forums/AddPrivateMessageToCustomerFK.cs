using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:50:37:3669696")]
    public class AddPrivateMessageToCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.PrivateMessageTable,
                nameof(PrivateMessage.ToCustomerId),
                nameof(Customer),
                nameof(Customer.Id));
        }

        #endregion
    }
}