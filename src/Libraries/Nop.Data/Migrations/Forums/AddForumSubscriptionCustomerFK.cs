using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:47:18:7699848")]
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