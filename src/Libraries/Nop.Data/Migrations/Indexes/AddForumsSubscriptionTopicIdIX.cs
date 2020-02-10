using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037701")]
    public class AddForumsSubscriptionTopicIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Forums_Subscription_TopicId", NopMappingDefaults.ForumsSubscriptionTable,
                i => i.Ascending(), nameof(ForumSubscription.TopicId));
        }

        #endregion
    }
}