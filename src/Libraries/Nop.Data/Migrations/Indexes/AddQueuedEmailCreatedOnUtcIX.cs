using FluentMigrator;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037687")]
    public class AddQueuedEmailCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_QueuedEmail_CreatedOnUtc").OnTable(nameof(QueuedEmail))
                .OnColumn(nameof(QueuedEmail.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}