using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:48:30:1910241")]
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