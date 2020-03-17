using FluentMigrator;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037690")]
    public class AddNewsletterSubscriptionEmailStoreIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_NewsletterSubscription_Email_StoreId").OnTable(nameof(NewsLetterSubscription))
                .OnColumn(nameof(NewsLetterSubscription.Email)).Ascending()
                .OnColumn(nameof(NewsLetterSubscription.StoreId)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}