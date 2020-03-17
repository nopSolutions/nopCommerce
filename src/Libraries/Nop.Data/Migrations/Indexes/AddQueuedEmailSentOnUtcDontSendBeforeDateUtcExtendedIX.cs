using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 12:02:35:9280393")]
    public class AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended").OnTable(nameof(QueuedEmail))
                .OnColumn(nameof(QueuedEmail.SentOnUtc)).Ascending()
                .OnColumn(nameof(QueuedEmail.DontSendBeforeDateUtc)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(QueuedEmail.SentTries));
        }

        #endregion
    }
}