using FluentMigrator;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.Messages
{
    [Migration(637097797031655781)]
    public class AddQueuedEmailEmailAccountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(QueuedEmail))
                .ForeignColumn(nameof(QueuedEmail.EmailAccountId))
                .ToTable(nameof(EmailAccount))
                .PrimaryColumn(nameof(EmailAccount.Id));

            Create.Index().OnTable(nameof(QueuedEmail)).OnColumn(nameof(QueuedEmail.EmailAccountId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}