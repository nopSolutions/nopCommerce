using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647926")]
    public class AddActivityLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_ActivityLog_CreatedOnUtc", nameof(ActivityLog), i => i.Descending(),
                nameof(ActivityLog.CreatedOnUtc));
        }

        #endregion
    }
}