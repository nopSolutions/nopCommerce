using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123537559280393)]
    public class AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended", nameof(QueuedEmail),
                    i => i.Ascending(), nameof(QueuedEmail.SentOnUtc), nameof(QueuedEmail.DontSendBeforeDateUtc))
                .Include(nameof(QueuedEmail.SentTries));
        }

        #endregion
    }
}