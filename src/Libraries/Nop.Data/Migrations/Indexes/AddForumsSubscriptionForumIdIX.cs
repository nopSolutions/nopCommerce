using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037700)]
    public class AddForumsSubscriptionForumIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Subscription_ForumId", NopMappingDefaults.ForumsSubscriptionTable,
                i => i.Ascending(), nameof(ForumSubscription.ForumId));
        }

        #endregion
    }
}