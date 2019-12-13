using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097784463004326)]
    public class AddForumPostCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsPostTable
                , nameof(ForumPost.CustomerId)
                , nameof(Customer)
                , nameof(Customer.Id));
        }

        #endregion
    }
}