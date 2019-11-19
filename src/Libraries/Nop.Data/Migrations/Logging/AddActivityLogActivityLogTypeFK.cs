using FluentMigrator;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.Logging
{
    [Migration(637097794508380329)]
    public class AddActivityLogActivityLogTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ActivityLog))
                .ForeignColumn(nameof(ActivityLog.ActivityLogTypeId))
                .ToTable(nameof(ActivityLogType))
                .PrimaryColumn(nameof(ActivityLogType.Id));
        }

        #endregion
    }
}