using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097788387699848)]
    public class AddForumSubscriptionCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsSubscriptionTable,
                nameof(ForumSubscription.CustomerId),
                nameof(Customer),
                nameof(Customer.Id));
        }

        #endregion
    }
}