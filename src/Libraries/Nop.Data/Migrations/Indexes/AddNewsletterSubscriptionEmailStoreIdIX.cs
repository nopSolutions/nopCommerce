using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037690)]
    public class AddNewsletterSubscriptionEmailStoreIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_NewsletterSubscription_Email_StoreId", nameof(NewsLetterSubscription), i => i.Ascending(),
                nameof(NewsLetterSubscription.Email), nameof(NewsLetterSubscription.StoreId));
        }

        #endregion
    }
}