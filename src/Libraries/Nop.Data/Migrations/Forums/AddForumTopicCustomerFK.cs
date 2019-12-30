using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097789101910241)]
    public class AddForumTopicCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsTopicTable,
                nameof(ForumTopic.CustomerId),
                nameof(Customer),
                nameof(Customer.Id));
        }

        #endregion
    }
}