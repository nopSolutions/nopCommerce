using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037687")]
    public class AddQueuedEmailCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_QueuedEmail_CreatedOnUtc", nameof(QueuedEmail), i => i.Descending(),
                nameof(QueuedEmail.CreatedOnUtc));
        }

        #endregion
    }
}