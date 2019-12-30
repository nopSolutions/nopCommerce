using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Messages
{
    [Migration(637097797031655781)]
    public class AddQueuedEmailEmailAccountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(QueuedEmail),
                nameof(QueuedEmail.EmailAccountId),
                nameof(EmailAccount),
                nameof(EmailAccount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}