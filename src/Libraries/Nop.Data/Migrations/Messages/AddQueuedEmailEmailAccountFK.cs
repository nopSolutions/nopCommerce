using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Messages
{
    [NopMigration("2019/11/19 05:01:43:1655781")]
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