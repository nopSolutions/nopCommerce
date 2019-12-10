using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097784463004326)]
    public class AddForumPostCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumsPostTable)
                .ForeignColumn(nameof(ForumPost.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id))
                .OnDelete(Rule.None);

            Create.Index().OnTable(NopMappingDefaults.ForumsPostTable).OnColumn(nameof(ForumPost.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}