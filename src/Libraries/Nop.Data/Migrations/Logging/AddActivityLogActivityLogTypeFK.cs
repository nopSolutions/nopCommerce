using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Logging
{
    [NopMigration("2019/11/19 04:57:30:8380329")]
    public class AddActivityLogActivityLogTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ActivityLog),
                nameof(ActivityLog.ActivityLogTypeId),
                nameof(ActivityLogType),
                nameof(ActivityLogType.Id),
                Rule.Cascade);
        }

        #endregion
    }
}