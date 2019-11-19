using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097788387699848)]
    public class AddForumSubscriptionCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumsSubscriptionTable)
                .ForeignColumn(nameof(ForumSubscription.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}