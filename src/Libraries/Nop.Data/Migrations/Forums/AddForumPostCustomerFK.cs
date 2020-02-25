using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:40:46:3004326")]
    public class AddForumPostCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsPostTable,
                nameof(ForumPost.CustomerId),
                nameof(Customer),
                nameof(Customer.Id));
        }

        #endregion
    }
}