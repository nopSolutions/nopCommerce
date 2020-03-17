using FluentMigrator;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647926")]
    public class AddActivityLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_ActivityLog_CreatedOnUtc").OnTable(nameof(ActivityLog))
                .OnColumn(nameof(ActivityLog.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}